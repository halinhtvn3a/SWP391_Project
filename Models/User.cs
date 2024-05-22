using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? RoleId { get; set; }

    public decimal? Balance { get; set; }

    public string? Qrcode { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();
}
