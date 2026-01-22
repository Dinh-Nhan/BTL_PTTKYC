using backend.Data;
using backend.Models;
using backend.Repository.interfaces;

namespace backend.Repository.implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public User CreateStaff(User staff)
        {
            var createdUser = _context.Users.Add(staff);
            
            _context.SaveChanges();
            return createdUser.Entity;
        }

        public bool DeleteStaff(int userId)
        {
            var user = getById(userId);
            if (user == null)
                return false;
            _context.Users.Remove(user);
            return _context.SaveChanges() > 0;
        }

        public List<User> getAllStaff()
        {
            return _context.Users
                .Where(s => s.RoleId == true)
                .ToList();
        }

        public User? getById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        public User Update(User user)
        {
            var updatedUser =_context.Users.Update(user);
            _context.SaveChanges();
            return updatedUser.Entity;
        }

        public User? ValidateUser(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return null;

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(
                password,
                user.PasswordHashing
            );

            if (!isValidPassword)
                return null;

            return user;
        }

    }
}
