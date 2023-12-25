using System;
using System.Collections.Generic;

namespace PropertyRentalManagement.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public string? SenderId { get; set; }

    public string? ReceiverId { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int? ReadStatusId { get; set; }

    public virtual Status? ReadStatus { get; set; }

    public virtual AppUser? Receiver { get; set; }

    public virtual AppUser? Sender { get; set; }
}
