using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class BillRepository : IBillRepository
    {
        private readonly AppDbContext _context;
        public BillRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Bill>> GetAll()
        {
            return await _context.Bills.ToListAsync();
        }
    }
}
