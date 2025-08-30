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

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime Start { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime End { get; set; }

    public string? Location { get; set; }

    public string UserId { get; set; } = null!;
}
