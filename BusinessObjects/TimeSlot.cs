using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class TimeSlot
{
    public string SlotId { get; set; } = null!;

    public string? CourtId { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly SlotStartTime { get; set; }

    public TimeOnly SlotEndTime { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Court? Court { get; set; }
}
