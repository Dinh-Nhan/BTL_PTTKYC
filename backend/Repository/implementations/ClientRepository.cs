using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Response;

namespace backend.Repository.implementations
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool DeleteClient(int clientId)
        {
            var client = _context.Clients.Find(clientId);
            if(client == null) return false;

            _context.Clients.Remove(client!);
            return _context.SaveChanges() > 0;
        }

        public async Task<List<ClientResponse>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .Select(c => new ClientResponse
                {
                    ClientId = c.ClientId,
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
            return clients;
        }

        public async Task<List<ClientResponse>?> GetClientByFullNameOrEmailAsync(string information)
        {
            if (string.IsNullOrWhiteSpace(information))
                return null;

            return await _context.Clients
                .Where(c =>
                    c.FullName.Contains(information) ||
                    c.Email.Substring(0, c.Email.IndexOf("@"))
                        .Contains(information)
                )
                .Select(c => new ClientResponse
                {
                    FullName = c.FullName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email
                })
                .ToListAsync();
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
