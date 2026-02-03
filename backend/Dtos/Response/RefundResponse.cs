namespace backend.Dtos.Response
{
    public class RefundResponse
    {
        public int BookingId { get; set; }
        public decimal OriginalDepositAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public int RefundPercentage { get; set; }
        public string RefundStatus { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime RefundDate { get; set; }
        public int DaysBeforeCheckIn { get; set; }
        public string? VnpTransactionNo { get; set; }
    }
}
