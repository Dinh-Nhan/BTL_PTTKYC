using backend.Data;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ActiveUser(int userId)
        {
            var user = _context.User.Find(userId);
            if (user == null)
            {
                return false;
            }
            user.IsActive = true;
            _context.SaveChanges();
            return true;
        }

        public async Task<User> CreateEmployee(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public bool DeactiveUser(int userId)
        {
            var user = _context.User.Find(userId);
            if (user == null)
            {
                return false;
            }
            user.IsActive = false;
            _context.SaveChanges();
            return true;
        }

        public bool DeleteEmployee(int userId)
        {
            var user = _context.User.Find(userId);
            if (user == null)
            {
                return false;
            }
            _context.User.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public Task<List<User>> GetAllUser()
        {
            var users = _context.User.ToListAsync();
            return users;
        }

        public async Task<List<UserResponse>?> SearchEmployeeByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return null;
            }

            var users = await _context.User
                .Where(u => u.FullName.Contains(fullName) && u.RoleId == true)
                .Select(u => new UserResponse
                {
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                }).ToListAsync();

            return users;
        }

        public async Task<List<UserResponse>> SearchUserByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return null;
            }

            var users = await _context.User
                .Where(u => u.FullName.Contains(fullName))
                .Select(u => new UserResponse
                {
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Gender = u.Gender,
                    DateOfBirth = u.DateOfBirth,
                    RoleId = u.RoleId,
                    IsActive = u.IsActive
                }).ToListAsync();

            return users;
        }
        public async Task<User?> UpdateEmployee(User user)
        {
            var existingUser = await _context.User.FindAsync(user.UserId);

            if (existingUser == null)
                return null;

            // update các field được phép
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;

            await _context.SaveChangesAsync();
            return existingUser;
        }


    }
}
