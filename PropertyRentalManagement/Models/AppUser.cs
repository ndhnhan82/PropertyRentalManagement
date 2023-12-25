using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyRentalManagement.Models;

public partial class AppUser : IdentityUser
{
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }
    [Display(Name = "Last Name")]

    public string? LastName { get; set; }

    public string? Address { get; set; }

    public byte[]? ProfilePicture { get; set; }
    public string? UserStatus { get; set; }

    public virtual ICollection<Apartment> Apartments { get; set; } = new List<Apartment>();

    public virtual ICollection<Appointment> AppointmentPotentialTenants { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentPropertyManagers { get; set; } = new List<Appointment>();

    public virtual ICollection<Building> Buildings { get; set; } = new List<Building>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

}
