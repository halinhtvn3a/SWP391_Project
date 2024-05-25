using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Branch
{
    public string BranchId { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Description { get; set; }

    public string Picture { get; set; } = null!;

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public string OpenDay { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Court> Courts { get; set; } = new List<Court>();
}
