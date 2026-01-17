using backend.Dtos.Response;

namespace backend.Service.interfaces
{
    public interface IClientService
    {
        Task<ApiResponse<List<ClientResponse>>> GetClientByFullNameOrEmailAsync(string information);
        ApiResponse<bool> DeleteClient(int clientId);
    }
}
