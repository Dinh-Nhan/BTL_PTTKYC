using backend.Dtos.Response;
using backend.Service.interfaces;

namespace backend.Service.implementations
{
    public class ApiResponseFactory : IApiResponseFactory
    {
        // api response cho trường hợp fail
        public ApiResponse<T> Fail<T>(int statusCode = 500, string message = "Error")
        {
            return new ApiResponse<T>(
                    statusCode: statusCode,
                    result: default,
                    message: message
                );
        }

        public ApiResponse<T> Success<T>(T data, string message = "Successfully")
        {
            return new ApiResponse<T>(
                    result: data,
                    message: message,
                    statusCode: StatusCodes.Status200OK
                );
        }
    }
}
