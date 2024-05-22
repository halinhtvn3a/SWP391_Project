using System;
using System.Collections.Generic;

namespace CourtManagement.Models;

public partial class Payment
{
    public string PaymentId { get; set; } = null!;

    public string Method { get; set; } = null!;

    public string? BookingId { get; set; }

    public DateTime Paytime { get; set; }

    public string? ExternalVnPayTransactionCode { get; set; }

    public virtual Booking? Booking { get; set; }
}
