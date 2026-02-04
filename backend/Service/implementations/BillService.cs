using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IApiResponseFactory _apiResponseFactory;
        public BillService(IBillRepository billRepository, IApiResponseFactory apiResponseFactory)
        {
            _billRepository = billRepository;
            _apiResponseFactory = apiResponseFactory;
        }
        public async Task<ApiResponse<List<Bill>>> GetAll()
        {
            var bills = await _billRepository.GetAll();
            if (bills != null)
            {
                return _apiResponseFactory.Success(bills);
            }
            else
            {
                return _apiResponseFactory.Fail<List<Bill>>(
                        StatusCodes.Status404NotFound,
                        "No bills found"
                    );
            }
        }
    }
}
