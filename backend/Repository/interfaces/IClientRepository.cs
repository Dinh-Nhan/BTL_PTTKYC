using backend.Dtos.Response;
using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IClientRepository
    {
        Client createClient(Client client);
        Task<Client?> GetByEmail (string email);
        Task Update(Client existingClient);
        Task<List<ClientResponse>?> GetClientByFullNameOrEmailAsync(string information);
        bool DeleteClient(int clientId);
        Task<List<ClientResponse>> GetAllClientsAsync();
    }
}
