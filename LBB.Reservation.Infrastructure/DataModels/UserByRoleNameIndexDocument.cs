using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Keyless]
[Table("UserByRoleNameIndex_Document")]
[Index("UserByRoleNameIndexId", "DocumentId", Name = "IDX_FK_UserByRoleNameIndex_Document")]
public partial class UserByRoleNameIndexDocument
{
    [Column(TypeName = "BIGINT")]
    public long UserByRoleNameIndexId { get; set; }

    [Column(TypeName = "BIGINT")]
    public long DocumentId { get; set; }
}
