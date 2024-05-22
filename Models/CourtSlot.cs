using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class CourtSlot
{
    public string CourtslotId { get; set; } = null!;

    public string? CourtId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Starttime { get; set; }

    public TimeOnly Endtime { get; set; }

    public decimal Price { get; set; }

    public bool Status { get; set; }

    public string? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Court? Court { get; set; }
}
