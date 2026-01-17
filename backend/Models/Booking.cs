using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int ClientId { get; set; }

    public int RoomId { get; set; }

    public DateTime CheckInDatetime { get; set; }

    public DateTime CheckOutDatetime { get; set; }

    public int NumberDay { get; set; }

    public int AdultCount { get; set; }

    public int? ChildCount { get; set; }

    public string? Status { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? DepositAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bill> Bill { get; set; } = new List<Bill>();

    public virtual Client Client { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
