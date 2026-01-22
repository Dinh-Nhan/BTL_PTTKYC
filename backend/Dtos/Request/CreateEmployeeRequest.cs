using backend.Models;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class CreateEmployeeRequest
    {
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public string Email { get; set; } = null!;

        public string PasswordHashing { get; set; } = null!;

        public string FullName { get; set; } = null!;

        [Phone(ErrorMessage = "Phone number is invalid!")]
        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public bool? RoleId { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
