using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Service.interfaces
{
    public interface IUserService
    {
        ApiResponse<User> validateUser(LoginRequest request);
        List<User> getAllStaff();

        User CreateStaff(User staff);
        User Update(User user);
        bool DeleteStaff(int userId);

    }
}
