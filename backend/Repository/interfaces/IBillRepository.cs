using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IBillRepository
    {
        Task<List<Bill>> GetAll(); 
    }
}
