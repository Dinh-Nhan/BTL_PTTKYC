using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IUserRepository
    {
        Task<List<UserResponse>?> SearchEmployeeByFullName(string fullName);
        Task<List<UserResponse>> SearchUserByFullName(string fullName);
        bool DeleteEmployee(int userId);
        Task<User> CreateEmployee(User user);
        Task<User> UpdateEmployee(User user);
        Task<List<User>> GetAllUser();
        bool DeactiveUser(int userId);
        bool ActiveUser(int userId);
    }
}
