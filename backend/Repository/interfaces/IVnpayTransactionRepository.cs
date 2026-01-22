using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IVnpayTransactionRepository
    {
        Task<bool> AddVnpayTransaction(VnpayTransaction vnpayTransaction);

        Task<VnpayTransaction?> getVnpayTransactionByBookingId(int bookingId);
    }
}
