using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IClientRepository
    {
        Client createClient(Client client);
        Task<Client?> GetByEmail (string email);
        Task Update(Client existingClient);
    }
}
