using backend.Dtos.Response;

namespace backend.Repository.interfaces
{
    public interface IClientRepository
    {
        Task<List<ClientResponse>?> GetClientByFullNameOrEmailAsync(string information);
        bool DeleteClient(int clientId);
        Task<List<ClientResponse>> GetAllClientsAsync();
    }
}
