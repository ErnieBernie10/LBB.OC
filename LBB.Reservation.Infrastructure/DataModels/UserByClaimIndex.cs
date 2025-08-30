using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("UserByClaimIndex")]
[Index("DocumentId", Name = "IDX_FK_UserByClaimIndex")]
[Index("DocumentId", "ClaimType", "ClaimValue", Name = "IDX_UserByClaimIndex_DocumentId")]
public partial class UserByClaimIndex
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "BIGINT")]
    public long? DocumentId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }
}
