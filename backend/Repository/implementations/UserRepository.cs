using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using backend.Dtos.Response;
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

        public bool ActiveUser(int userId)
        {
            var user = _context.Users.Find(userId);
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
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public bool DeactiveUser(int userId)
        {
            var user = _context.Users.Find(userId);
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
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return false;
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return true;
        }

        public Task<List<User>> GetAllUser()
        {
            var users = _context.Users.ToListAsync();
            return users;
        }

        public async Task<List<UserResponse>?> SearchEmployeeByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return null;
            }

            var users = await _context.Users
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

            var users = await _context.Users
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
            var existingUser = await _context.Users.FindAsync(user.UserId);

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
