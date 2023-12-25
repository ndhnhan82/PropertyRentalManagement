using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Building
{
    public int BuildingId { get; set; }

    public string? Name { get; set; }

    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    public string? PropertyOwnerId { get; set; }

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual AppUser? PropertyOwner { get; set; }
}
