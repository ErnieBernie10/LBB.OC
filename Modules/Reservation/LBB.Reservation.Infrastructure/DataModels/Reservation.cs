using System;
using System.Collections.Generic;

namespace LBB.Reservation.Infrastructure.DataModels;

public partial class Reservation
{
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int AttendeeCount { get; set; }

    public int SessionId { get; set; }

    public string Reference { get; set; } = null!;

    public DateTime? ConfirmationSentOn { get; set; }

    public DateTime? CancelledOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public virtual Session Session { get; set; } = null!;
}
