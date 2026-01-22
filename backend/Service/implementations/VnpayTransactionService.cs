using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class VnpayTransactionService : IVnpayTransactionService
    {
        private readonly IVnpayTransactionRepository _vnpayTransactionRepository;
        private readonly ILogger<VnpayTransactionService> _logger;
        public VnpayTransactionService(IVnpayTransactionRepository vnpayTransactionRepository, ILogger<VnpayTransactionService> logger)
        {
            _vnpayTransactionRepository = vnpayTransactionRepository;
            _logger = logger;
        }
        // hàm nội bộ không trả về gì, nên khi lỗi throw exception để stop program (quá trình dev)
        public async Task<bool> addVnpayTransaction(VnpayTransaction vnpayTransaction)
        {
            if(vnpayTransaction == null)
            {
                throw new ArgumentNullException(nameof(vnpayTransaction));
            }
            
            var result = await _vnpayTransactionRepository.AddVnpayTransaction(vnpayTransaction);
            
            if(!result)
            {
                throw new Exception("Failed to add Vnpay transaction");
            }
            _logger.LogInformation("Add Vnpay transaction successfully");
            return result;
        }

        public async Task<VnpayTransaction?> getByBookingId(int? bookingId)
        {
            if (bookingId <= 0 || !bookingId.HasValue)
                throw new InvalidDataException("booking ID invalid");

            var result = await _vnpayTransactionRepository.getVnpayTransactionByBookingId(bookingId.Value);

            if (result == null)
                throw new NullReferenceException("not exist vnpay transaction have this bookingId");

            _logger.LogInformation("Get vnpay transaction by bookingId successfully");
            return result;
        }
    }
}
