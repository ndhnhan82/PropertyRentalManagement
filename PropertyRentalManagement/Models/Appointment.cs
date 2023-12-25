using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public string? PropertyManagerId { get; set; }

    public string? PotentialTenantId { get; set; }

    public DateTime ScheduledDate { get; set; }

    public string Description { get; set; } = null!;

    public int? StatusId { get; set; }

    public virtual AppUser? PotentialTenant { get; set; }

    public virtual AppUser? PropertyManager { get; set; }

    public virtual Status? Status { get; set; }
}
