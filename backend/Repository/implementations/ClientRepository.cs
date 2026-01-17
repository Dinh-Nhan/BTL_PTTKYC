using backend.Data;
using backend.Dtos.Response;
using backend.Repository.interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(AppDbContext context, ILogger<ClientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool DeleteClient(int clientId)
        {
            var client = _context.Client.Find(clientId);
            if(client == null) return false;

            _context.Client.Remove(client!);
            return _context.SaveChanges() > 0;
        }

        public async Task<List<ClientResponse>?> GetClientByFullNameOrEmailAsync(string information)
        {
            if (string.IsNullOrWhiteSpace(information))
                return null;

            return await _context.Client
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

    }
}
