using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("UserByRoleNameIndex")]
[Index("RoleName", Name = "IDX_UserByRoleNameIndex_RoleName")]
public partial class UserByRoleNameIndex
{
    [Key]
    public int Id { get; set; }

    public string? RoleName { get; set; }

    [Column(TypeName = "INT")]
    public int? Count { get; set; }
}
