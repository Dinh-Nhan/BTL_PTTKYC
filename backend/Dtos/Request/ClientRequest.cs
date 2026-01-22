using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class ClientRequest
    {
        public string FullName { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [EmailAddress(ErrorMessage = "The email is not in the correct format")]
        public string Email { get; set; } = null!;
    }
}
