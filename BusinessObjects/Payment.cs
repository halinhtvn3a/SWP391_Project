using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string? BookingId { get; set; }

    public decimal PaymentAmount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public string? PaymentMessage { get; set; }

    public string? PaymentStatus { get; set; }

    public string? PaymentSignature { get; set; }

    public virtual Booking? Booking { get; set; }
}
