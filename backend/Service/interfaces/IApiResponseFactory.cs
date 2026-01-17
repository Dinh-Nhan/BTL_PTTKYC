using backend.Dtos.Response;

namespace backend.Service.interfaces
{
    public interface IApiResponseFactory
    {
        public ApiResponse<T> Success<T>(T data, string message = "Successfully");
        public ApiResponse<T> Fail<T>(int statusCode, string message = "Error");

    }
}
