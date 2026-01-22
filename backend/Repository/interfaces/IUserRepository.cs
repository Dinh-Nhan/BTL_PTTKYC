using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IUserRepository
    {

        User? ValidateUser(string email,string password);
        User? getById(int userId);

        List<User> getAllStaff();

        // chỉ có thể tạo staff - admin đã được cố định
        User CreateStaff(User staff);
        User Update(User user);
        bool DeleteStaff(int userId);

    }
}
