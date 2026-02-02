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

                // 2. Lấy booking và transaction từ DB
                var booking = await _bookingRepository.GetByIdWithDetails(request.BookingId);
                if (booking == null)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status404NotFound,
                        $"Booking {request.BookingId} not found"
                    );
                }

                var vnpayTransaction = await _vnpayTransactionService.getByBookingId(request.BookingId);
                if (vnpayTransaction == null)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status404NotFound,
                        $"No payment transaction found for booking {request.BookingId}"
                    );
                }

                // 3. Validate refund eligibility
                var validation = ValidateRefundEligibility(booking, vnpayTransaction);
                if (!validation.IsValid)
                {
                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status400BadRequest,
                        validation.ErrorMessage
                    );
                }

                // 4. Query VNPay trước để đảm bảo giao dịch vẫn hợp lệ
                //var queryResult = await _vnpayService.QueryTransaction(
                //    vnpayTransaction,
                //    $"Booking{booking.BookingId}",
                //    ipAddress
                //);

                //if (queryResult == null || queryResult.vnp_ResponseCode != "00")
                //{
                //    return _apiResponseFactory.Fail<RefundResponse>(
                //        StatusCodes.Status500InternalServerError,
                //        $"Cannot verify transaction status: {queryResult?.vnp_Message}"
                //    );
                //}

                //// Kiểm tra trạng thái giao dịch
                //if (queryResult.vnp_TransactionStatus != "00")
                //{
                //    return _apiResponseFactory.Fail<RefundResponse>(
                //        StatusCodes.Status400BadRequest,
                //        $"Transaction is not successful. Status: {queryResult.vnp_TransactionStatus}"
                //    );
                //}

                // 5. Gửi yêu cầu hoàn tiền tới VNPay
                var refundResult = await _vnpayService.RefundTransaction(
                    vnpayTransaction,
                    $"Refund for Booking{booking.BookingId} - Reason: {request.Reason}",
                    request.RequestedBy ?? "System",
                    ipAddress
                );

                if (refundResult == null || refundResult.vnp_ResponseCode != "00")
                {
                    _logger.LogWarning(
                        "Refund failed for booking {BookingId}. VNPay response: {Response}",
                        request.BookingId,
                        refundResult?.vnp_Message
                    );

                    return _apiResponseFactory.Fail<RefundResponse>(
                        StatusCodes.Status400BadRequest,
                        $"Refund request failed: {refundResult?.vnp_Message}"
                    );
                }

                // 6. Cập nhật database
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    // Cập nhật booking
                    booking.Status = "REFUNDED";
                    booking.PaymentStatus = "REFUNDED";
                    booking.Note = $"Hoàn tiền 100% - Lý do: {request.Reason} - By: {request.RequestedBy}";
                    booking.UpdatedAt = DateTime.UtcNow;
                    await _bookingRepository.Update(booking);

                    // Cập nhật room về AVAILABLE
                    var room = _roomRepository.getById(booking.RoomId);
                    if (room != null && room.Status == "BOOKED")
                    {
                        UpdateRoomStatus(room, "AVAILABLE");
                        _roomRepository.updateRoom(room);
                    }

                    // Lưu thông tin hoàn tiền vào transaction
                    vnpayTransaction.VnpTransactionStatus = refundResult.vnp_TransactionStatus;
                    vnpayTransaction.PaymentStatus = "REFUNDED";
                    //await _vnpayTransactionService.UpdateTransaction(vnpayTransaction);

                    await _unitOfWork.CommitTransactionAsync();

                    _logger.LogInformation(
                        "Refund successful for booking {BookingId}. Amount: {Amount}. VNPay Transaction: {TxnNo}",
                        request.BookingId,
                        vnpayTransaction.VnpAmount,
                        refundResult.vnp_TransactionNo
                    );

                    var response = new RefundResponse
                    {
                        BookingId = request.BookingId,
                        RefundAmount = vnpayTransaction.VnpAmount,
                        RefundStatus = "SUCCESS",
                        Message = "Refund processed successfully. Money will be returned within 3-5 business days.",
                        VnpTransactionNo = refundResult.vnp_TransactionNo
                    };

                    return _apiResponseFactory.Success(response, "Refund processed successfully");
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    _logger.LogError(
                        ex,
                        "Error updating database after successful refund for booking {BookingId}",
                        request.BookingId
                    );

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for booking {BookingId}", request?.BookingId);

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
        #endregion
    }
}