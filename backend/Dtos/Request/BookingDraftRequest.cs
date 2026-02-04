namespace backend.Dtos.Request
{
    public class BookingDraftRequest
    {
        public int RoomId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adult { get; set; }
        public int Child { get; set; }
    }
}