using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class Comment
{
    public string CommentId { get; set; } = null!;

    public string? ReplyText { get; set; }

    public DateTime? DateReplied { get; set; }

    public string? UserId { get; set; }

    public string? CourtSlotId { get; set; }

    public virtual CourtSlot? CourtSlot { get; set; }

    public virtual User? User { get; set; }
}
