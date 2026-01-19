namespace backend.Dtos.Response
{
    public class UserResponse
    {

        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public bool? IsActive { get; set; }

    }
}
