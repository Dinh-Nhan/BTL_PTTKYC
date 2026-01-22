using backend.Dtos.Request;
using backend.Dtos.Response;
using backend.Models;

namespace backend.Service.interfaces
{
    public interface IJwtService
    {
        ApiResponse<LoginResponse> GenerateToken(LoginRequest request);
        ApiResponse<LoginResponse> RefreshToken();
        ApiResponse<bool> Introspect(IntrospectRequest request);
        ApiResponse<bool> Logout();

        ApiResponse<UserResponse> getMe();
    }
}
