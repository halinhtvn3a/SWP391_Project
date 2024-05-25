using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? RoleId { get; set; }

    public decimal? Balance { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? FullName { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }
}
