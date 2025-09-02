using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

public partial class Session
{
    [Key]
    public int Id { get; set; }

    public int Type { get; set; }

    public int Capacity { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Column(TypeName = "DATETIME")]
    public DateTime Start { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime End { get; set; }

    public string Location { get; set; } = null!;

    public string UserId { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
