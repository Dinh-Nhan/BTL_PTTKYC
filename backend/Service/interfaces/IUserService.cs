using backend.Dtos.Request;
using backend.Dtos.Response;

namespace backend.Service.interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserResponse>>> SearchEmployeeByFullName(string fullName);
        Task<ApiResponse<bool>> DeleteEmployee(int userId);
        Task<ApiResponse<UserResponse>> CreateEmployee(CreateEmployeeRequest user);
        Task<ApiResponse<UserResponse>> UpdateEmployee(int id, UpdateEmployeeRequest user);
        Task<ApiResponse<List<UserResponse>>> GetAllUser();
        Task<ApiResponse<List<UserResponse>>> SearchUserByFullName(string fullName);
        Task<ApiResponse<bool>> DeactiveUser(int userId);
        Task<ApiResponse<bool>> ActiveUser(int userId);
    }
}
