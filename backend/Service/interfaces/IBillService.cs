using backend.Dtos.Response;
using backend.Models;

namespace backend.Service.interfaces
{
    public interface IBillService
    {
        Task<ApiResponse<List<Bill>>> GetAll();
    }
}
