using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Response
{
    public class BookingResponse
    {
        public ClientResponse clientResponse { get; set; } = null!;

        public int bookingId { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDatetime { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOutDatetime { get; set; }

        public int NumberDay { get; set; }

        public int AdultCount { get; set; }

        public int? ChildCount { get; set; }

        public decimal? DepositAmount { get; set; }

        public string? Note { get; set; }

        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }


        public RoomResponse roomResponse { get; set; } = null!;

    }
}
