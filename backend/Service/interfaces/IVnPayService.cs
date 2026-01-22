using backend.Models;
using backend.Service.implementations;

namespace backend.Service.interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(long amount, string orderInfo, string ipAddress);
        VnpayTransaction? ProcessPaymentCallback(IQueryCollection queryParams);
        string createQueryUrl(VnpayTransaction vnpayTransaction, string orderInfo, string ipAddress);
    }
}
