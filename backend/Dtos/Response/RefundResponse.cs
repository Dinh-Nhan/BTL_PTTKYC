namespace backend.Dtos.Response
{
    public class RefundResponse
    {
        public int BookingId { get; set; }
        public decimal RefundAmount { get; set; }
        public string RefundStatus { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string VnpTransactionNo { get; set; } = string.Empty;
    }
}
