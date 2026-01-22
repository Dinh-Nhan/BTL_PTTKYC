using AutoMapper;
using Azure.Core;
using backend.Data.UnitOfWork;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;
using System.Net;
using System.Net.WebSockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace backend.Service.implementations
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly IApiResponseFactory _apiResponseFactory;
        private readonly ILogger<BookingService> _logger;
        private readonly IVnPayService _vnpayService;
        private readonly IVnpayTransactionService _vnpayTransactionService;

        public BookingService(
            IUnitOfWork unitOfWork,
            IBookingRepository bookingRepository,
            IRoomRepository roomRepository,
            IClientRepository clientRepository,
            IMapper mapper,
            IApiResponseFactory apiResponseFactory,
            ILogger<BookingService> logger,
            IVnPayService vnpayService,
            IVnpayTransactionService vnpayTransactionService)
        {
            _unitOfWork = unitOfWork;
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _clientRepository = clientRepository;
            _mapper = mapper;
            _apiResponseFactory = apiResponseFactory;
            _logger = logger;
            _vnpayService = vnpayService;
            _vnpayTransactionService = vnpayTransactionService;
        }

        public async Task<ApiResponse<PaymentResponse>> CreateBookingForClient(BookingRequest request, string ipAddress)
        {
            _logger.LogInformation("IPV4 Address: {ip}", ipAddress);
            try
            {

                if (request.Client == null)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        "Thông tin khách hàng là bắt buộc"
                    );
                }

                var dateValidation = ValidateBookingDates(request); 
                if (!dateValidation.IsValid)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        dateValidation.ErrorMessage!
                    );
                }
    

                if (request.DepositAmount.HasValue && request.DepositAmount.Value < 0)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        "Số tiền đặt cọc không hợp lệ"
                    );
                }


                var room =  _roomRepository.GetByIdWithRoomType(request.RoomId);
                if (room == null)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status404NotFound,
                        $"Không tìm thấy phòng với ID {request.RoomId}"
                    );
                }

                var roomValidation = ValidateRoom(room, request.AdultCount, request.ChildCount);
                if (!roomValidation.IsValid)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        roomValidation.ErrorMessage!
                    );
                }


                var isAvailable = await _bookingRepository.IsRoomAvailable(
                    request.RoomId,
                    request.CheckInDatetime,
                    request.CheckOutDatetime
                );

                if (!isAvailable)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status409Conflict,
                        "Phòng đã được đặt trong khoảng thời gian này"
                    );
                }


                decimal roomPrice = room.RoomType?.BasePrice ?? 0;
                decimal totalAmount = CalculateTotalAmount(roomPrice, request.NumberDay);

                if (request.DepositAmount.HasValue && request.DepositAmount.Value > totalAmount)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        "Số tiền đặt cọc không được lớn hơn tổng tiền phòng"
                    );
                }


                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var client = await GetOrCreateClient(request.Client);

                    var booking = CreateBookingEntity(request, client.ClientId, totalAmount);

                    var createdBooking = await _bookingRepository.createBooking(booking);

                    if (createdBooking == null)
                    {
                        throw new InvalidOperationException("Không thể tạo booking");
                    }

                    //UpdateRoomStatus(room, "BOOKED");

                    //_roomRepository.updateRoom(room);

                    // tạo url thanh toán vnpay
                    string paymentUrl = _vnpayService.CreatePaymentUrl(
                        Convert.ToInt64(createdBooking.DepositAmount ?? 0),
                        $"Booking{createdBooking.BookingId}",
                        ipAddress
                    );



                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation(
                        "Booking created successfully. BookingId: {BookingId}, ClientId: {ClientId}, RoomId: {RoomId}",
                        createdBooking.BookingId,
                        client.ClientId,
                        room.RoomId
                    );

                    var response = new PaymentResponse
                    {
                        PaymentUrl = paymentUrl
                    };

                    // chỉ trả về url thanh toán
                    return _apiResponseFactory.Success(response, "Đặt phòng thành công");
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogError(
                        ex,
                        "Error creating booking for room {RoomId}. Client: {Email}",
                        request.RoomId,
                        request.Client?.Email
                    );

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateBookingForClient");

                return _apiResponseFactory.Fail<PaymentResponse>(
                    StatusCodes.Status500InternalServerError,
                    $"Đã xảy ra lỗi khi tạo booking: {ex.Message}"
                );
            }
        }


        private (bool IsValid, string? ErrorMessage) ValidateBookingDates(BookingRequest request)
        {
            if (request.CheckInDatetime.Date >= request.CheckOutDatetime.Date)
            {
                return (false, "The check-out date must be after the check-in date");
            }

            if (request.CheckInDatetime.Date < DateTime.UtcNow.Date)
            {
                return (false, "The check-in date cannot be a date from the past");
            }

            var actualDays = (request.CheckOutDatetime.Date - request.CheckInDatetime.Date).Days;
            if (request.NumberDay != actualDays)
            {
                return (false, $"The number of days in ({request.NumberDay}) does not match the time period ({actualDays} days)");
            }

            // Maximum booking duration
            if (request.NumberDay > 30)
            {
                return (false, "The booking period must not exceed 30 days");
            }

            return (true, null);
        }

        private (bool IsValid, string? ErrorMessage) ValidateRoom(Room room, int adultCount, int? childCount)
        {
            if (room.IsActive == false)
            {
                return (false, "The room is currently unavailable.");
            }

            if (room.Status != "AVAILABLE")
            {
                return (false, $"The room is currently in the status: {room.Status}");
            }

            var totalGuests = adultCount + (childCount ?? 0);
            var totalGuestAllow = (room.RoomType?.MaxAdult ?? 0) + (room.RoomType?.MaxChildren ?? 0);
            if (room.RoomType?.MaxAdult != null && room.RoomType?.MaxChildren != null && totalGuests > totalGuestAllow)
            {
                return (false, $"The total number of guests ({totalGuests}) exceeds the maximum capacity ({totalGuestAllow})");
            }

            return (true, null);
        }

        private decimal CalculateTotalAmount(decimal pricePerNight, int numberOfDays)
        {
            return pricePerNight * numberOfDays;
        }

        private Booking CreateBookingEntity(BookingRequest request, int clientId, decimal totalAmount)
        {

            var booking = _mapper.Map<Booking>(request);
            booking.ClientId = clientId;
            booking.TotalPrice = totalAmount;
            booking.Status = "PENDING";
            booking.PaymentStatus = request.DepositAmount.HasValue && request.DepositAmount.Value > 0
                    ? "PARTIAL"
                    : "UNPAID";
            booking.CreatedAt = DateTime.UtcNow;

            return booking;         
        }

        private void UpdateRoomStatus(Room room, string status)
        {
            room.Status = status;
            room.UpdatedAt = DateTime.UtcNow;
        }     


        private async Task<Client> GetOrCreateClient(ClientRequest clientRequest)
        {
            // Database query - cần async
            var existingClient = await _clientRepository.GetByEmail(clientRequest.Email);

            if (existingClient != null)
            {
                _logger.LogInformation("Using existing client: {Email}", existingClient.Email);

                if (IsClientInfoChanged(existingClient, clientRequest))
                {
                    UpdateClientInfo(existingClient, clientRequest);
                    await _clientRepository.Update(existingClient); 
                    _logger.LogInformation("Updated client info: {Email}", existingClient.Email);
                }

                return existingClient;
            }


            var newClient = _mapper.Map<Client>(clientRequest);   

            var createdClient =  _clientRepository.createClient(newClient);

            if (createdClient == null)
            {
                throw new InvalidOperationException("Unable to generate customer information");
            }

            _logger.LogInformation("Created new client: {Email}", createdClient.Email);

            return createdClient;
        }

        private bool IsClientInfoChanged(Client existing, ClientRequest request)
        {
            return existing.FullName != request.FullName ||
                   existing.PhoneNumber != request.PhoneNumber;
        }

        private void UpdateClientInfo(Client existing, ClientRequest request)
        {
            existing.FullName = request.FullName;
            existing.PhoneNumber = request.PhoneNumber;
        }


        public async Task<ApiResponse<List<BookingResponse>>> GetAllBookings()
        {
            try
            {
                // Database query - async
                var bookings = await _bookingRepository.GetAllWithDetails();

                var response = bookings.Select(b =>
                {
                    var room = _mapper.Map<RoomResponse>(b.Room);
                    var booking = _mapper.Map<BookingResponse>(b);
                    booking.roomResponse = room;
                    return booking;

                }).ToList();

                return _apiResponseFactory.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bookings");

                return _apiResponseFactory.Fail<List<BookingResponse>>(
                    StatusCodes.Status500InternalServerError,
                    $"Error retrieving booking list: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<BookingResponse>> GetBookingById(int bookingId)
        {
            try
            {
                // Database query - async
                var booking = await _bookingRepository.GetByIdWithDetails(bookingId);

                if (booking == null)
                {
                    return _apiResponseFactory.Fail<BookingResponse>(
                        StatusCodes.Status404NotFound,
                        $"No booking found with ID {bookingId}"
                    );
                }

                // Mapping - sync
                var response = _mapper.Map<BookingResponse>(booking);
                response.roomResponse = _mapper.Map<RoomResponse>(booking.Room);

                return _apiResponseFactory.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking {BookingId}", bookingId);

                return _apiResponseFactory.Fail<BookingResponse>(
                    StatusCodes.Status500InternalServerError,
                    $"Error retrieving booking information: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<bool>> CancelBooking(int bookingId, CancelBookingRequest request)
        {
            try
            {
                // Database query - async
                var booking = await _bookingRepository.GetById(bookingId);

                if (booking == null)
                {
                    return _apiResponseFactory.Fail<bool>(
                        StatusCodes.Status404NotFound,
                        $"No booking found with ID {bookingId}"
                    );
                }

                // Validation - sync
                var validation = ValidateCancellation(booking);
                if (!validation.IsValid)
                {
                    return _apiResponseFactory.Fail<bool>(
                        StatusCodes.Status400BadRequest,
                        validation.ErrorMessage!
                    );
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    booking.Status = "CANCELLED";
                    booking.UpdatedAt = DateTime.UtcNow;
                    booking.Note = $"ĐÃ HỦY - Lí do: {request.reasonCancel}";
                    await _bookingRepository.Update(booking); 

                    var room = _roomRepository.getById(booking.RoomId);
                    if (room != null)
                    {
                        UpdateRoomStatus(room, "AVAILABLE");
                        _roomRepository.updateRoom(room);
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation("Booking {BookingId} cancelled successfully", bookingId);

                    return _apiResponseFactory.Success(true, "Booking cancelled successfully");
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);

                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    $"Error when canceling booking: {ex.Message}"
                );
            }
        }

        private (bool IsValid, string? ErrorMessage) ValidateCancellation(Booking booking)
        {
            if (booking.Status == "CANCELLED")
            {
                return (false, "The booking was previously cancelled.");
            }

            if (booking.Status == "CHECKED_OUT")
            {
                return (false, "Bookings that have been completed cannot be canceled.");
            }

            if (booking.CheckInDatetime.Date < DateTime.UtcNow.Date)
            {
                return (false, "Bookings cannot be canceled after the check-in date.");
            }

            return (true, null);
        }

        public async Task<ApiResponse<BookingResponse>> ProcessPaymentCallback(VnpayTransaction vnpayTransaction)
        {
            try
            {
                var booking = await _bookingRepository.GetById(vnpayTransaction.BookingId);

                if (booking == null)
                {
                    return _apiResponseFactory.Fail<BookingResponse>(
                        StatusCodes.Status404NotFound,
                        $"No booking found with ID {vnpayTransaction.BookingId}"
                    );
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    if (vnpayTransaction.VnpResponseCode.Equals("00"))
                    {

                        booking.PaymentStatus = "PARTIAL"; 
                        booking.Status = "CONFIRMED";
                        booking.UpdatedAt = DateTime.UtcNow;
                        await _bookingRepository.Update(booking);

                        var room = _roomRepository.getById(booking.RoomId);
                        if (room != null)
                        {
                            UpdateRoomStatus(room, "BOOKED");
                            _roomRepository.updateRoom(room);
                        }

                        // sau khi cập nhật booking và room thành công thì mới lưu giao dịch vnpay
                        await _vnpayTransactionService.addVnpayTransaction(vnpayTransaction);

                        await _unitOfWork.CommitTransactionAsync();

                        _logger.LogInformation(
                            "Payment successful. Booking {BookingId} confirmed. Room {RoomId} status updated to BOOKED",
                            booking.BookingId,
                            booking.RoomId
                        );
                        var response = _mapper.Map<BookingResponse>(booking);
                        response.roomResponse = _mapper.Map<RoomResponse>(room);
                        response.clientResponse = _mapper.Map<ClientResponse>(booking.Client);

                        _logger.LogInformation("Response for client: {response}", response);
                        return _apiResponseFactory.Success(response, "Payment successful. Booking confirmed");
                    }
                    else
                    {
                        // ❌ Thanh toán thất bại

                        booking.Status = "CANCELLED"; // Hủy booking
                        booking.PaymentStatus = "FAILED";
                        booking.Note = $"Payment failed: VNPAY payment error";
                        booking.UpdatedAt = DateTime.UtcNow;
                        await _bookingRepository.Update(booking);

                        await _unitOfWork.CommitTransactionAsync();

                        _logger.LogWarning(
                            "Payment failed for booking {BookingId}. Booking cancelled.",
                            booking.BookingId
                        );

                        return _apiResponseFactory.Fail<BookingResponse>(
                            StatusCodes.Status400BadRequest,
                            $"Payment failed: VNPAY payment error"
                        );
                    }
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment callback for booking {BookingId}", vnpayTransaction.BookingId);

                return _apiResponseFactory.Fail<BookingResponse>(
                    StatusCodes.Status500InternalServerError,
                    $"Payment processing error: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<QueryTransactionResponse>> createQuerydrTransaction(int bookingId, string ipAddress)
        {
            if(bookingId <= 0 || string.IsNullOrEmpty(ipAddress))
            {
                return _apiResponseFactory.Fail<QueryTransactionResponse>(
                    StatusCodes.Status400BadRequest,
                    "Invalid data request"
                );
            }

            var booking = await _bookingRepository.GetById(bookingId);
            var vnpayTransaction = await _vnpayTransactionService.getByBookingId(bookingId);
            
            if(booking == null || vnpayTransaction == null)
            {
                return _apiResponseFactory.Fail<QueryTransactionResponse>(
                    StatusCodes.Status404NotFound,
                    "Booking or transaction not found"
                );
            }

            var queryResult = _vnpayService.createQueryUrl(
                        vnpayTransaction,
                        $"Booking{booking.BookingId}",
                        ipAddress
            );

            if(queryResult == null)
            {
                return _apiResponseFactory.Fail<QueryTransactionResponse>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to create query url"
                );
            }
            return _apiResponseFactory.Success(new QueryTransactionResponse { querydrUrl = queryResult}, "Query URL created successfully");
        }
    }
}