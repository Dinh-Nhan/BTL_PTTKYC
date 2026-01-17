using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal FinalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BillDetail> BillDetail { get; set; } = new List<BillDetail>();

    public virtual Booking Booking { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
