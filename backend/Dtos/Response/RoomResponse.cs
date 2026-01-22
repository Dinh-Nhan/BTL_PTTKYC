namespace backend.Dtos.Response
{
    public class RoomResponse
    {
        public int RoomId { get; set; }

        public int RoomTypeId { get; set; }

        public string RoomNumber { get; set; } = null!;

        public int Floor { get; set; }

        public string Building { get; set; } = null!;

        public string? Status { get; set; }

        public string? Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public RoomTypeResponse roomType { get; set; } = null!;
    }
}
