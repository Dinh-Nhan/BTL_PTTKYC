namespace backend.Dtos.Response
{
    public class ApiResponse<T>
    {
        public int statusCode {get; set;} = 200;
        public string message { get; set; } = string.Empty;
        public T? result { get; set; }

        public ApiResponse(int statusCode, string message, T? result = default)
        {
            this.statusCode = statusCode;
            this.result = result;
            this.message = message;
        }
    }
}
