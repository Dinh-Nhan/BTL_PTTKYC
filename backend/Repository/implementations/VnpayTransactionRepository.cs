using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class VnpayTransactionRepository : IVnpayTransactionRepository
    {
        private readonly AppDbContext _context;
        public VnpayTransactionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddVnpayTransaction(VnpayTransaction vnpayTransaction)
        {
            await _context.VnpayTransactions.AddAsync(vnpayTransaction);
            var created = await _context.SaveChangesAsync();
            return created > 0;
        }

        public async Task<VnpayTransaction?> getVnpayTransactionByBookingId(int bookingId)
        {
            var result = await  _context.VnpayTransactions
                .FirstOrDefaultAsync(v => v.BookingId == bookingId);
            return result;
        }
    }
}
