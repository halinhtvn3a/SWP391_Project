using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Review
{
    public string ReviewId { get; set; } = null!;

    public string? ReviewText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public int? Rating { get; set; }

    public string? UserId { get; set; }

    public string? CourtId { get; set; }

    public virtual Court? Court { get; set; }

    public virtual User? User { get; set; }
}
