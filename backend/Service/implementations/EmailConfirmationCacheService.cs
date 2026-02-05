using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;

namespace backend.Service.implementations
{
    public class EmailConfirmationCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<EmailConfirmationCacheService> _logger;

        public EmailConfirmationCacheService(
            IMemoryCache cache,
            ILogger<EmailConfirmationCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public string GenerateConfirmationToken(string email)
        {
            try
            {
                _logger.LogInformation("=== GENERATE TOKEN START ===");
                _logger.LogInformation($"Email: {email}");

                // Generate random token
                var randomBytes = RandomNumberGenerator.GetBytes(32);
                var token = Convert.ToBase64String(randomBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');

                _logger.LogInformation($"Generated token: {token}");
                _logger.LogInformation($"Token length: {token.Length}");

                // Create cache key
                var cacheKey = GetCacheKey(token);
                _logger.LogInformation($"Cache key: {cacheKey}");

                // Save to cache
                _cache.Set(
                    cacheKey,
                    email,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                        Priority = CacheItemPriority.High
                    }
                );

                // 🔍 Verify it was saved
                var testRetrieve = _cache.Get<string>(cacheKey);
                _logger.LogInformation($"Verification - Email saved in cache: {testRetrieve}");

                if (testRetrieve != email)
                {
                    _logger.LogError($"❌ Cache save FAILED! Expected: {email}, Got: {testRetrieve}");
                    throw new Exception("Failed to save token in cache");
                }

                _logger.LogInformation("✅ Token generated and cached successfully");
                _logger.LogInformation("=== GENERATE TOKEN END ===");

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error generating confirmation token");
                throw;
            }
        }

        public string? VerifyConfirmationToken(string token)
        {
            try
            {
                _logger.LogInformation("=== VERIFY TOKEN START ===");
                _logger.LogInformation($"Input token: [{token}]");
                _logger.LogInformation($"Token length: {token?.Length ?? 0}");

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Token is null or whitespace");
                    return null;
                }

                var cacheKey = GetCacheKey(token.Trim());
                _logger.LogInformation($"Cache key: {cacheKey}");

                // Try to get from cache
                if (!_cache.TryGetValue(cacheKey, out string? email))
                {
                    _logger.LogWarning($"❌ Token NOT found in cache");
                    _logger.LogWarning($"Cache key attempted: {cacheKey}");
                    return null;
                }

                _logger.LogInformation($"✅ Token found! Email: {email}");

                // Remove token after verification (one-time use)
                _cache.Remove(cacheKey);
                _logger.LogInformation("Token removed from cache (one-time use)");

                _logger.LogInformation("=== VERIFY TOKEN END ===");

                return email;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error verifying confirmation token");
                return null;
            }
        }

        // 🔍 Debug method - kiểm tra xem token có trong cache không (không xóa)
        public bool TokenExistsInCache(string token)
        {
            try
            {
                var cacheKey = GetCacheKey(token);
                var exists = _cache.TryGetValue(cacheKey, out string? email);

                _logger.LogInformation($"Token exists check: {exists}, Email: {email}");

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token existence");
                return false;
            }
        }

        // 🔍 Debug method - lấy email mà không xóa token
        public string? PeekToken(string token)
        {
            try
            {
                var cacheKey = GetCacheKey(token);
                _cache.TryGetValue(cacheKey, out string? email);
                return email;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error peeking token");
                return null;
            }
        }

        private string GetCacheKey(string token)
            => $"EMAIL_CONFIRM_{token}";
    }
}