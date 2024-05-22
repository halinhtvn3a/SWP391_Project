using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class Court
{
    public string CourtId { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public string Picture { get; set; } = null!;

    public TimeOnly Opentime { get; set; }

    public TimeOnly Closetime { get; set; }

    public string Openday { get; set; } = null!;

    public virtual ICollection<CourtSlot> CourtSlots { get; set; } = new List<CourtSlot>();
}
