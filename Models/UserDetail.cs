using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class UserDetail
{
    public string UserDetailId { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Fullname { get; set; }

    public string? UserId { get; set; }

    public virtual User? User { get; set; }
}
