using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public int MaxAdult { get; set; }

    public int? MaxChildren { get; set; }

    public decimal? RoomArea { get; set; }

    public bool? AllowPet { get; set; }

    public decimal BasePrice { get; set; }

    public decimal? ExtraAdultFee { get; set; }

    public decimal? ExtraChildFee { get; set; }

    public string? Amenities { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
