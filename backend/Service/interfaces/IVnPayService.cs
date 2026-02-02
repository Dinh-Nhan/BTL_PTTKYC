using backend.Dtos.Response;
using backend.Models;
using backend.Service.implementations;

namespace backend.Service.interfaces
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(long amount, string orderInfo, string ipAddress);
        VnpayTransaction? ProcessPaymentCallback(IQueryCollection queryParams);
        //string createQueryUrl(VnpayTransaction vnpayTransaction, string orderInfo, string ipAddress);

        Task<VnpayQueryResponse> QueryTransaction(VnpayTransaction vnpayTransaction, string orderInfo, string ipAddress);

        Task<VnpayRefundResponse> RefundTransaction(VnpayTransaction vnpayTransaction, string orderInfo, string createdBy, string ipAddress);

    }
}
