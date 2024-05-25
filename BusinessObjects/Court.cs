using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Court
{
    public string CourtId { get; set; } = null!;

    public string? BranchId { get; set; }

    public string? CourtName { get; set; }

    public bool Status { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();
}
