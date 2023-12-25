using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();
}
