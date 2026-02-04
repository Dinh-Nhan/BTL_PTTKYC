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
            try
            {

                if (request.Client == null)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        "Customer information is required"
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
    
                var room =  _roomRepository.GetByIdWithRoomType(request.RoomId);
                if (room == null)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status404NotFound,
                        $"No rooms found matching ID {request.RoomId}"
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
                        "The room has been booked for this period."
                    );
                }

                var today = DateTime.Now.Date;
                var checkInDate = request.CheckInDatetime.Date;

                var daysDiff = (checkInDate - today).TotalDays;

                if (daysDiff < 3)
                {
                    return _apiResponseFactory.Fail<PaymentResponse>(
                        StatusCodes.Status400BadRequest,
                        "Please book your room at least 3 days in advance"
                    );
                }


                // tự động tính số ngày ở
                int numberOfDays = CalculateNumberOfDays(request.CheckInDatetime, request.CheckOutDatetime);

                decimal roomPrice = room.RoomType?.BasePrice ?? 0;

                // tính tổng tiền
                decimal totalAmount = CalculateTotalAmount(roomPrice, numberOfDays);

                // tính tiền cọc
                decimal depositAmount = CalculateDepositAmount(totalAmount);

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    var client = await GetOrCreateClient(request.Client);

                    var booking = CreateBookingEntity(request, client.ClientId, totalAmount, depositAmount, numberOfDays);

                    var createdBooking = await _bookingRepository.createBooking(booking);

                    if (createdBooking == null)
                    {
                        throw new InvalidOperationException("Unable to create a booking.");
                    }


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
                    return _apiResponseFactory.Success(response, "Booking successful");
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
                    $"An error occurred while creating the booking.: {ex.Message}"
                );
            }
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
                    booking.Status = "cancelled";
                    booking.UpdatedAt = DateTime.UtcNow;
                    booking.Note = $"ĐÃ HỦY - Lí do: {request.reasonCancel}";
                     _bookingRepository.Update(booking); 

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

        

        public async Task<ApiResponse<BookingResponse>> ProcessPaymentCallback(VnpayTransaction vnpayTransaction)
        {
            try
            {
                //if(vnpayTransaction == null)
                //{
                //    return _apiResponseFactory.Fail<BookingResponse>(
                //        StatusCodes.Status404NotFound,
                //        $"the payment is cancelled!"
                //    );
                //}
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
                         _bookingRepository.Update(booking);

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
                         _bookingRepository.Update(booking);

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

        public async Task<ApiResponse<VnpayQueryResponse>> QueryTransactionStatus(int bookingId, string ipAddress)
        {
            try
            {
                if (bookingId <= 0 || string.IsNullOrEmpty(ipAddress))
                {
                    return _apiResponseFactory.Fail<VnpayQueryResponse>(
                        StatusCodes.Status400BadRequest,
                        "Invalid data request"
                    );
                }

                // 1. Lấy booking và transaction từ DB
                var booking = await _bookingRepository.GetById(bookingId);
                if (booking == null)
                {
                    return _apiResponseFactory.Fail<VnpayQueryResponse>(
                        StatusCodes.Status404NotFound,
                        $"Booking {bookingId} not found"
                    );
                }

                var vnpayTransaction = await _vnpayTransactionService.getByBookingId(bookingId);
                if (vnpayTransaction == null)
                {
                    return _apiResponseFactory.Fail<VnpayQueryResponse>(
                        StatusCodes.Status404NotFound,
                        $"No payment transaction found for booking {bookingId}"
                    );
                }

                // 2. Gửi query request tới VNPay
                var queryResult = await _vnpayService.QueryTransaction(
                    vnpayTransaction,
                    $"Booking{booking.BookingId}",
                    ipAddress
                );

                if (queryResult == null || queryResult.vnp_ResponseCode != "00")
                {
                    return _apiResponseFactory.Fail<VnpayQueryResponse>(
                        StatusCodes.Status500InternalServerError,
                        $"Failed to query transaction: {queryResult?.vnp_Message}"
                    );
                }

                // 3. Cập nhật trạng thái nếu có thay đổi
                if (queryResult.vnp_TransactionStatus != vnpayTransaction.VnpTransactionStatus)
                {
                    await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        vnpayTransaction.VnpTransactionStatus = queryResult.vnp_TransactionStatus;
                        //await _vnpayTransactionService.UpdateTransaction(vnpayTransaction);

                        await _unitOfWork.CommitTransactionAsync();

                        _logger.LogInformation(
                            "Updated transaction status for booking {BookingId}: {Status}",
                            bookingId,
                            queryResult.vnp_TransactionStatus
                        );
                    }
                    catch
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        throw;
                    }
                }

                return _apiResponseFactory.Success(queryResult, "Transaction status retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying transaction status for booking {BookingId}", bookingId);
                return _apiResponseFactory.Fail<VnpayQueryResponse>(
                    StatusCodes.Status500InternalServerError,
                    $"Error querying transaction: {ex.Message}"
                );
            }
        }

        // Mock refund 
        public async Task<ApiResponse<RefundResponse>> RefundBooking(RefundRequest request, string ipAddress)
        {
            try
            {
                // 1. Validate input
                if (request == null || request.BookingId <= 0)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status400BadRequest,
                        "Invalid refund request"
                    );
                }

                // 2. Lấy booking từ DB
                var booking = await _bookingRepository.GetByIdWithDetails(request.BookingId);
                if (booking == null)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status404NotFound,
                        $"Booking {request.BookingId} not found"
                    );
                }

                // 3. Lấy transaction (nếu có)
                var vnpayTransaction = await _vnpayTransactionService.getByBookingId(request.BookingId);

                // 4. Validate refund eligibility (kiểm tra cơ bản)
                var basicValidation = ValidateBasicRefundEligibility(booking);
                if (!basicValidation.IsValid)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status400BadRequest,
                        basicValidation.ErrorMessage
                    );
                }

                // 5. Tính số ngày còn lại đến check-in
                int daysUntilCheckIn = CalculateDaysUntilCheckIn(booking.CheckInDatetime);

                // 6. Tính % hoàn tiền theo chính sách
                var refundPolicy = CalculateRefundPolicy(daysUntilCheckIn);

                if (!refundPolicy.IsEligible)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status400BadRequest,
                        refundPolicy.Message
                    );
                }

                decimal originalDepositAmount = booking.DepositAmount ?? 0;
                decimal refundAmount = CalculateRefundAmount(originalDepositAmount, refundPolicy.RefundPercentage);

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // Cập nhật booking
                    booking.Status = "CANCELLED"; 
                    booking.PaymentStatus = "REFUNDED";
                    booking.Note = $"[MOCK REFUND] Hoàn {refundPolicy.RefundPercentage}% ({FormatCurrency(refundAmount)}) - " +
                                  $"Lý do: {request.Reason} - " +
                                  $"Số ngày trước check-in: {daysUntilCheckIn} - " +
                                  $"By: {request.RequestedBy ?? "System"}";
                    booking.UpdatedAt = DateTime.UtcNow;
                     //_bookingRepository.Update(booking);

                    // Cập nhật room về AVAILABLE
                    var room = _roomRepository.getById(booking.RoomId);
                    if (room != null && (room.Status == "BOOKED" || room.Status == "RESERVED"))
                    {
                        UpdateRoomStatus(room, "AVAILABLE");
                        _roomRepository.updateRoom(room);
                    }

                    // Cập nhật transaction nếu có
                    if (vnpayTransaction != null)
                    {
                        vnpayTransaction.PaymentStatus = "REFUNDED";
                        vnpayTransaction.VnpTransactionStatus = "02";
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation(
                        "[MOCK REFUND] Booking {BookingId} refunded successfully. " +
                        "Days before check-in: {Days}, Refund %: {Percentage}%, Amount: {Amount}",
                        request.BookingId,
                        daysUntilCheckIn,
                        refundPolicy.RefundPercentage,
                        refundAmount
                    );

                    var response = new RefundResponse
                    {
                        BookingId = request.BookingId,
                        OriginalDepositAmount = originalDepositAmount,
                        RefundAmount = refundAmount,
                        RefundPercentage = refundPolicy.RefundPercentage,
                        RefundStatus = "SUCCESS",
                        Message = $"[MOCK] Hoàn tiền thành công {refundPolicy.RefundPercentage}% " +
                                 $"({FormatCurrency(refundAmount)}). " +
                                 $"Số tiền sẽ được hoàn vào tài khoản trong vòng 3-5 ngày làm việc.",
                        RefundDate = DateTime.UtcNow,
                        DaysBeforeCheckIn = daysUntilCheckIn,
                        VnpTransactionNo = vnpayTransaction?.VnpTransactionNo ?? $"MOCK-{Guid.NewGuid().ToString().Substring(0, 8)}"
                    };

                    return _apiResponseFactory.Success(
                        response,
                        $"Hoàn tiền thành công {refundPolicy.RefundPercentage}%"
                    );
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogError(
                        ex,
                        "Error updating database after mock refund for booking {BookingId}",
                        request.BookingId
                    );

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing mock refund for booking {BookingId}", request?.BookingId);

                return _apiResponseFactory.Fail<RefundResponse>(
                    StatusCodes.Status500InternalServerError,
                    $"Error processing refund: {ex.Message}"
                );
            }
        }

        #region helper method
        private (bool IsValid, string ErrorMessage) ValidateRefundEligibility(
            Booking booking,
            VnpayTransaction transaction)
        {
            // 1. Kiểm tra trạng thái booking
            if (booking.Status == "REFUNDED")
            {
                return (false, "This booking has already been refunded");
            }

            if (booking.Status == "CANCELLED")
            {
                return (false, "This booking has been cancelled");
            }

            if (booking.Status == "CHECKED_OUT")
            {
                return (false, "Cannot refund completed bookings");
            }

            if (booking.PaymentStatus != "PARTIAL" && booking.PaymentStatus != "SUCCESS")
            {
                return (false, $"Cannot refund unpaid booking. Payment status: {booking.PaymentStatus}");
            }

            // 2. Kiểm tra trạng thái giao dịch
            if (transaction.PaymentStatus == "REFUNDED")
            {
                return (false, "Transaction has already been refunded");
            }

            // 3. Kiểm tra thời gian - CHỈ hoàn 100% nếu trước 3 ngày check-in
            var daysUntilCheckIn = (booking.CheckInDatetime.Date - DateTime.UtcNow.Date).Days;

            if (daysUntilCheckIn < 3)
            {
                return (false, $"Refund is only available 3 or more days before check-in. Days remaining: {daysUntilCheckIn}");
            }

            // 4. Kiểm tra đã checkin chưa
            if (booking.CheckInDatetime.Date <= DateTime.UtcNow.Date)
            {
                return (false, "Cannot refund after check-in date");
            }

            return (true, null);
        }
        private int CalculateNumberOfDays(DateTime checkInDate, DateTime checkOutDate)
        {
            TimeSpan duration = checkOutDate.Date - checkInDate.Date;
            int numberOfDays = (int)duration.TotalDays;

            return numberOfDays > 0 ? numberOfDays : 1;
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

            
            int numberOfDays = CalculateNumberOfDays(request.CheckInDatetime, request.CheckOutDatetime);
            
            if (numberOfDays <= 0)
            {
                return (false, "Invalid booking duration");
            }

            if (numberOfDays > 30)
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

        // booking đang xử lý và chưa được thanh toán
        private Booking CreateBookingEntity(BookingRequest request, int clientId, decimal totalPrice, decimal depositAmount, int numberOfDays)
        {

            var booking = _mapper.Map<Booking>(request);
            booking.ClientId = clientId;
            booking.TotalPrice = totalPrice;
            booking.DepositAmount = depositAmount;
            booking.NumberDay = numberOfDays;
            booking.Status = "pending";
            booking.CreatedAt = DateTime.UtcNow;
            booking.PaymentStatus = "unpaid";
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

            var createdClient = _clientRepository.createClient(newClient);

            if (createdClient == null)
            {
                throw new InvalidOperationException("Unable to generate customer information");
            }

            _logger.LogInformation("Created new client: {Email}", createdClient.Email);

            return createdClient;
        }

        private decimal CalculateDepositAmount(decimal totalAmount)
        {
            const decimal DEPOSIT_PERCENTAGE = 0.10m; // 10%
            decimal depositAmount = totalAmount * DEPOSIT_PERCENTAGE;

            // Làm tròn đến hàng nghìn (VNĐ)
            depositAmount = Math.Round(depositAmount / 1000) * 1000;

            return depositAmount;
        }

        // helper cho refund
        // kiểm tra điều kiện hoàn tiền cơ bản
        private (bool IsValid, string ErrorMessage) ValidateBasicRefundEligibility(Booking booking)
        {
            // 1. Kiểm tra trạng thái booking
            if (booking.Status == "REFUNDED")
            {
                return (false, "This booking has already been refunded.");
            }

            if (booking.Status == "CANCELLED")
            {
                return (false, "This booking has been cancelled.");
            }

            if (booking.Status == "CHECKED_OUT")
            {
                return (false, "No refunds are available for completed bookings.");
            }

            if (booking.Status == "CHECKED_IN")
            {
                return (false, "No refunds are available for bookings that have already been checked in.");
            }

            if (booking.PaymentStatus != "PARTIAL" && booking.PaymentStatus != "PAID")
            {
                return (false, $"No refunds are available for unpaid bookings. Status: {booking.PaymentStatus}");
            }

            if (!booking.DepositAmount.HasValue || booking.DepositAmount.Value <= 0)
            {
                return (false, "No deposit required for refund");
            }

            if (booking.CheckInDatetime.Date < DateTime.UtcNow.Date)
            {
                return (false, "No refunds are available after the check-in date.");
            }

            return (true, null);
        }

        // tính số ngày còn lại đến ngày check-in
        private int CalculateDaysUntilCheckIn(DateTime checkInDate)
        {
            DateTime today = DateTime.UtcNow.Date;
            DateTime checkIn = checkInDate.Date;

            TimeSpan difference = checkIn - today;
            return (int)difference.TotalDays;
        }

        private (bool IsEligible, int RefundPercentage, string Message) CalculateRefundPolicy(int daysUntilCheckIn)
        {
            if (daysUntilCheckIn >= 3)
            {
                //Từ 3 ngày trở lên: Hoàn 100%
                return (true, 100, "100% refund of deposit (from 3 days before check-in)");
            }
            else if (daysUntilCheckIn == 2)
            {
                // 2 ngày: Hoàn 50%
                return (true, 50, "50% of the deposit will be refunded (2 days before check-in)");
            }
            else if (daysUntilCheckIn == 1)
            {
                //1 ngày: Không hoàn tiền
                return (false, 0, "No refunds are possible (only 1 day left before check-in). According to policy, refund requests must be made at least 2 days in advance.");
            }
            else
            {
                //Đã qua hoặc đúng ngày check-in
                return (false, 0, "No refunds will be given (if you have arrived or passed the check-in date).");
            }
        }

        private decimal CalculateRefundAmount(decimal depositAmount, int refundPercentage)
        {
            if (refundPercentage <= 0 || refundPercentage > 100)
            {
                return 0;
            }

            decimal refundAmount = depositAmount * refundPercentage / 100;

            refundAmount = Math.Round(refundAmount / 1000) * 1000;

            return refundAmount;
        }

        //format tiền VNĐ
        private string FormatCurrency(decimal amount)
        {
            return $"{amount:N0} VNĐ";
        }

        public async Task<ApiResponse<bool>> UpdateDeposit(int bookingId, decimal deposit)
        {
            var result = await _bookingRepository.UpdateDeposit(bookingId, deposit);
            if (result)
            {
                return _apiResponseFactory.Success(true, "Deposit updated successfully");
            }
            else
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to update deposit or not found"
                );
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusById(int bookingId, string status)
        {
            var result = await  _bookingRepository.UpdateStatusById(bookingId, status);
            if (result)
            {
                return _apiResponseFactory.Success(true, "Status updated successfully");
            }
            else
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status500InternalServerError,
                    "Failed to update status or not found"
                );
            }
        }
        #endregion
    }
}

// chính sách hoàn tiền: 
//3 ngày trở lên: Hoàn 100%
//2 ngày: Hoàn 50%
//1 ngày hoặc ít hơn: Không hoàn tiền