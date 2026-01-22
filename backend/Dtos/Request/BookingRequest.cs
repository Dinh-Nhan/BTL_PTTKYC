using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Request
{
    public class BookingRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The room ID must be greater than 0.")]
        public int RoomId { get; set; }
       
        [Required]
        public ClientRequest Client { get; set; } = null!;

        [DataType(DataType.Date)]
        [Required]
        public DateTime CheckInDatetime { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime CheckOutDatetime { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The number of adults must be greater than or equal to 0.")]
        public int NumberDay { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The number of adults must be greater than 0")]
        public int AdultCount { get; set; }

        public int? ChildCount { get; set; }

        public decimal? DepositAmount { get; set; }

        public string? Note { get; set; }
    }
}
