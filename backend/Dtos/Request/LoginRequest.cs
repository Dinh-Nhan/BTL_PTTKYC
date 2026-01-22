using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "The email is not in the correct format")]
        public string Email { get; set; } = null!;
        [Required]
        [MinLength(8, ErrorMessage = "The password must have at least 8 characters")]
        public string Password { get; set; } = null!;
    }
}
