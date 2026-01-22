using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Service.implementations;

namespace backend.Service.interfaces
{
    public interface IBookingService
    {

        Task<ApiResponse<PaymentResponse>> CreateBookingForClient(BookingRequest request, string ipAddess);
        Task<ApiResponse<List<BookingResponse>>> GetAllBookings();
        Task<ApiResponse<BookingResponse>> GetBookingById(int bookingId);
        Task<ApiResponse<bool>> CancelBooking(int bookingId, CancelBookingRequest request);
        Task<ApiResponse<BookingResponse>> ProcessPaymentCallback(VnpayTransaction vnpayTransaction);

        Task<ApiResponse<QueryTransactionResponse>> createQuerydrTransaction(int bookingId, string ipAddress);
    }
}
