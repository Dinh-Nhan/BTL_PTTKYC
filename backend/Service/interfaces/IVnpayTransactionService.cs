using backend.Models;

namespace backend.Service.interfaces
{
    public interface IVnpayTransactionService
    {
        Task<bool> addVnpayTransaction(VnpayTransaction vnpayTransaction);
        Task<VnpayTransaction?> getByBookingId(int? bookingId);
    }
}
