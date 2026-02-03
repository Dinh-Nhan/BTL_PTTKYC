using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class RefundRequest
    {
        [Required]
        public int BookingId { get; set; } 
        public string Reason { get; set; } = string.Empty;

        public string RequestedBy { get; set; } = string.Empty; // Email hoặc tên người yêu cầu
    }
}
