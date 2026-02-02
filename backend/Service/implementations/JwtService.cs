using AutoMapper;
using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;
using backend.Repository.interfaces;
using backend.Service.interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Service.implementations
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly IApiResponseFactory _apiResponseFactory;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public JwtService(
            IConfiguration config,
            IApiResponseFactory apiResponseFactory,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            )
        {
            _config = config;
            _apiResponseFactory = apiResponseFactory;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }


        public ApiResponse<LoginResponse> GenerateToken(LoginRequest request)
        {
            var existingUser = _userRepository.ValidateUser(request.Email, request.Password);

            if (existingUser == null)
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status401Unauthorized,
                    "Invalid credentials"
                );
            }

            // Revoke all existing refresh tokens for this user
            _refreshTokenRepository.RevokeAllByUserId(existingUser.UserId);

            // Generate tokens
            var accessToken = GenerateAccessToken(existingUser);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                UserId = existingUser.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    double.Parse(_config["Jwt:RefreshTokenExpireDays"] ?? "7")
                ),
                CreatedAt = DateTime.UtcNow,
                Revoked = false
            };

            _refreshTokenRepository.Create(refreshTokenEntity);

            // Return response
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = int.Parse(_config["Jwt:ExpireMinutes"]!)
            };

            return _apiResponseFactory.Success(response);
        }

        private string GenerateAccessToken(User user)
        {
            string role = user.RoleId.HasValue && user.RoleId.Value ? "STAFF" : "ADMIN";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_config["Jwt:ExpireMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            const int maxRetries = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                var randomNumber = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);

                // Check if token already exists
                if (_refreshTokenRepository.GetByToken(token) == null)
                {
                    return token;
                }
            }

            throw new InvalidOperationException("Failed to generate unique refresh token after multiple attempts");
        }

        public ApiResponse<LoginResponse> RefreshToken()
        {
            var refreshToken = GetRefreshTokenFromHeader();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status400BadRequest,
                    "Refresh token is required"
                );
            }


            var storedToken = _refreshTokenRepository.GetByTokenForUpdate(refreshToken);

            if (storedToken == null)
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status401Unauthorized,
                    "Invalid refresh token"
                );
            }

            if (storedToken.Revoked.HasValue && storedToken.Revoked.Value)
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status401Unauthorized,
                    "Refresh token has been revoked"
                );
            }

            if (storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status401Unauthorized,
                    "Refresh token has expired"
                );
            }

            var user = _userRepository.getById(storedToken.UserId);
            if (user == null)
            {
                return _apiResponseFactory.Fail<LoginResponse>(
                    StatusCodes.Status401Unauthorized,
                    "User not found"
                );
            }

            storedToken.Revoked = true;
            _refreshTokenRepository.SaveChanges();

            // Generate new tokens
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefreshToken,
                UserId = user.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    double.Parse(_config["Jwt:RefreshTokenExpireDays"] ?? "7")
                ),
                CreatedAt = DateTime.UtcNow,
                Revoked = false
            };

            _refreshTokenRepository.Create(newRefreshTokenEntity);

            var response = new LoginResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = int.Parse(_config["Jwt:ExpireMinutes"]!)
            };

            return _apiResponseFactory.Success(response);
        }


        public ApiResponse<bool> Introspect()
        {
            string token = GetAccessTokenFromHeader()!;

            if (string.IsNullOrWhiteSpace(token))
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status400BadRequest,
                    "Token is required"
                );
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return _apiResponseFactory.Success(true);
            }
            catch (Exception ex)
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status401Unauthorized,
                    $"Invalid token: {ex.Message}"
                );
            }
        }

        /// Logout và revoke refresh token
        public ApiResponse<bool> Logout()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status401Unauthorized,
                    "User not authenticated"
                );
            }

            var refreshToken = GetRefreshTokenFromHeader();

            Console.WriteLine("refresh Token" + refreshToken);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return _apiResponseFactory.Fail<bool>(
                    StatusCodes.Status400BadRequest,
                    "Refresh token is required"
                );
            }

            var storedToken = _refreshTokenRepository.GetByTokenForUpdate(refreshToken);

            if (storedToken != null && storedToken.UserId.ToString() == userId)
            {
                storedToken.Revoked = true;
                _refreshTokenRepository.SaveChanges();
            }

            return _apiResponseFactory.Success(true, "Logout successful");
        }

        public ApiResponse<UserResponse> getMe()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return _apiResponseFactory.Fail<UserResponse>(
                    StatusCodes.Status400BadRequest,
                    "User not found"
                );
            int userId = int.Parse(currentUserId);

            return _apiResponseFactory.Success(
                _mapper.Map<UserResponse>(_userRepository.getById(userId))
                );
        }
        #region helper method
        private string? GetAccessTokenFromHeader()
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        private string? GetRefreshTokenFromHeader()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["X-Refresh-Token"].ToString();
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        private string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        }

        

        #endregion

    }
}