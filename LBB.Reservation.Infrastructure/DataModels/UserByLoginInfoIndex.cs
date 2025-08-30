using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("UserByLoginInfoIndex")]
[Index("DocumentId", Name = "IDX_FK_UserByLoginInfoIndex")]
[Index("DocumentId", "LoginProvider", "ProviderKey", Name = "IDX_UserByLoginInfoIndex_DocumentId")]
public partial class UserByLoginInfoIndex
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "BIGINT")]
    public long? DocumentId { get; set; }

    public string? LoginProvider { get; set; }

    public string? ProviderKey { get; set; }
}
