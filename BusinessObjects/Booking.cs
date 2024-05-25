using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string? UserId { get; set; }

    public string? SlotId { get; set; }

    public DateOnly BookingDate { get; set; }

    public bool Check { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual TimeSlot? Slot { get; set; }

    public virtual User? User { get; set; }
}
