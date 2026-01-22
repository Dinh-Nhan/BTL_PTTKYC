using backend.Models;

namespace backend.Dtos.Response
{
    public class UserResponse
    {
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public string GenderValue { get; set; } = "Male";

        public DateOnly DateOfBirth { get; set; }

        public bool? RoleId { get; set; }

        public string RoleName { get; set; } = "Employee";

        public bool? IsActive { get; set; }
        public string IsActiveStatus { get; set; } = "Active";

    }
}
