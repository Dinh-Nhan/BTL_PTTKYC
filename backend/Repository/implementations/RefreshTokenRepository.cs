using backend.Data;
using backend.Models;
using backend.Repository.interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repository.implementations
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public RefreshToken? GetByToken(string token)
        {
            return _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefault(rt => rt.Token == token);
        }


        public RefreshToken? GetByTokenForUpdate(string token)
        {
            return _context.RefreshTokens
                .FirstOrDefault(rt => rt.Token == token);
        }


        public void Create(RefreshToken refreshToken)
        {
            // Generate GUID nếu chưa có
            if (refreshToken.Id == Guid.Empty)
            {
                refreshToken.Id = Guid.NewGuid();
            }

            // Set CreatedAt nếu chưa có
            if (!refreshToken.CreatedAt.HasValue)
            {
                refreshToken.CreatedAt = DateTime.UtcNow;
            }

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();
        }


        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public void RevokeAllByUserId(int userId)
        {
            var activeTokens = _context.RefreshTokens
                .Where(rt => rt.UserId == userId && (!rt.Revoked.HasValue || !rt.Revoked.Value))
                .ToList();

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                {
                    token.Revoked = true;
                }

                _context.SaveChanges();
            }
        }
        public void DeleteExpiredTokens()
        {
            var expiredTokens = _context.RefreshTokens
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow)
                .ToList();

            if (expiredTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(expiredTokens);
                _context.SaveChanges();
            }
        }
    }
}