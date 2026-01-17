using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class BillDetail
{
    public int BillDetailId { get; set; }

    public int BillId { get; set; }

    public string ItemName { get; set; } = null!;

    public string ItemType { get; set; } = null!;

    public int? Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Description { get; set; }

    public virtual Bill Bill { get; set; } = null!;
}
