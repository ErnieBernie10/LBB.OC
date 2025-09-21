using System;
using System.Collections.Generic;

namespace LBB.Reservation.Infrastructure.DataModels;

public partial class Session
{
    public int Id { get; set; }

    public int Type { get; set; }

    public int Capacity { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public string Location { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public DateTime? CancelledOn { get; set; }

    public string? CancelledReason { get; set; }

    public string? CancelledBy { get; set; }

    public DateTime? PublishedOn { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
