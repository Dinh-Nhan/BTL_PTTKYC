using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class SendEmailConfirmationRequest
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty!;
        public string draftId { get; set; } = string.Empty!;
        public int roomId { get; set; }

    }
}
