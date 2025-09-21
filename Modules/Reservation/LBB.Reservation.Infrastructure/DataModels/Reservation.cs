using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Index("Reference", IsUnique = true)]
public partial class Reservation
{
    [Key]
    public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int AttendeeCount { get; set; }

    public int SessionId { get; set; }

    public string Reference { get; set; } = null!;

    [ForeignKey("SessionId")]
    [InverseProperty("Reservations")]
    public virtual Session Session { get; set; } = null!;

    public bool ConfirmationSent { get; set; }
}
