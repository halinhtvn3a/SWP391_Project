using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string? UserId { get; set; }

    public DateOnly Bookingdate { get; set; }

    public int Totalprice { get; set; }

    public bool Check { get; set; }

    public virtual ICollection<CourtSlot> CourtSlots { get; set; } = new List<CourtSlot>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
