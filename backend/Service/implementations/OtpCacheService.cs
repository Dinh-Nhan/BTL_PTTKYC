using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;

namespace backend.Service.implementations
{
    public class OtpCacheService
    {
        private readonly IMemoryCache _cache;

        public OtpCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();
        }

        public void SaveOtp(string email, string otp)
        {
            var hash = HashOtp(otp);

            _cache.Set(
                GetCacheKey(email),
                hash,
                TimeSpan.FromMinutes(5)
            );
        }

        public bool VerifyOtp(string email, string otp)
        {
            if (!_cache.TryGetValue(GetCacheKey(email), out string? cachedHash))
                return false;

            if (cachedHash != HashOtp(otp))
                return false;

            _cache.Remove(GetCacheKey(email)); // OTP dùng 1 lần
            return true;
        }

        private string GetCacheKey(string email)
            => $"OTP_{email}";

        private string HashOtp(string otp)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(otp))
            );
        }
    }
}
