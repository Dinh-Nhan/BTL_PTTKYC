using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }



        public Client createClient(Client client)
        {
            var createdClient = _context.Clients.Add(client);
            _context.SaveChanges();
            return createdClient.Entity;
        }

        public async Task<Client?> GetByEmail(string email)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Email.Equals(email));
        }

        public async Task Update(Client existingClient)
        {
            await _context.SaveChangesAsync();
        }

    }
}
