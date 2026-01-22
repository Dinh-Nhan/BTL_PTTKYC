using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class VnpayTransaction
{
    public int VnpayId { get; set; }

    public int BookingId { get; set; }

    public int? BillId { get; set; }

    public string VnpTxnRef { get; set; } = null!;

    public string VnpTransactionNo { get; set; } = null!;

    public string VnpPayDate { get; set; } = null!;

    public long VnpAmount { get; set; }

    public string? VnpCurrencyCode { get; set; }

    public string VnpResponseCode { get; set; } = null!;

    public string VnpTransactionStatus { get; set; } = null!;

    public string? VnpMessage { get; set; }

    public string? VnpBankCode { get; set; }

    public string? VnpCardType { get; set; }

    public string VnpSecureHash { get; set; } = null!;

    public bool IsValidSignature { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bill? Bill { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
