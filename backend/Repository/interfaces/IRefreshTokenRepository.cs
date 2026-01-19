using backend.Models;

namespace backend.Repository.interfaces
{
    public interface IRefreshTokenRepository
    {
        RefreshToken? GetByToken(string token);
        RefreshToken? GetByTokenForUpdate(string token); // Thêm method mới
        void SaveChanges();

        void Create(RefreshToken refreshToken);
        void RevokeAllByUserId(int userId); // Thêm method này

    }
}
