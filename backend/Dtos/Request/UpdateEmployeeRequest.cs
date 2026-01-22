using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class UpdateEmployeeRequest
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Id is required!")]
        [EmailAddress(ErrorMessage = "Email is invalid!")]
        public string Email { get; set; } = null!;

        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required!")]
        [Phone(ErrorMessage = "Phone number is invalid!")]
        public string? PhoneNumber { get; set; }

        public bool? Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
