﻿using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Status
{
    public int StatusId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
