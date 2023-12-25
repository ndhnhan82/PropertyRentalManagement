using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Apartment
{
    public int ApartmentId { get; set; }

    public int? BuildingId { get; set; }

    public int ApartmentNumber { get; set; }

    public string Description { get; set; } = null!;

    public decimal Rent { get; set; }

    public int? StatusId { get; set; }

    public int? SizeId { get; set; }

    public string? TenantId { get; set; }

    public virtual Building? Building { get; set; }

    public virtual Size? Size { get; set; }

    public virtual Status? Status { get; set; }

    public virtual AppUser? Tenant { get; set; }
}
