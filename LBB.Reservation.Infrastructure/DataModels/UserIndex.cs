using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("UserIndex")]
[Index("DocumentId", Name = "IDX_FK_UserIndex")]
[Index("DocumentId", "UserId", "NormalizedUserName", "NormalizedEmail", "IsEnabled", Name = "IDX_UserIndex_DocumentId")]
[Index("DocumentId", "IsLockoutEnabled", "LockoutEndUtc", "AccessFailedCount", Name = "IDX_UserIndex_Lockout")]
public partial class UserIndex
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "BIGINT")]
    public long? DocumentId { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? NormalizedEmail { get; set; }

    [Column(TypeName = "BOOL")]
    public bool IsEnabled { get; set; }

    [Column(TypeName = "BOOL")]
    public bool IsLockoutEnabled { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime? LockoutEndUtc { get; set; }

    [Column(TypeName = "INT")]
    public int AccessFailedCount { get; set; }

    public string? UserId { get; set; }
}
