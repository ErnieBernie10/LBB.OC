using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("Document")]
[Index("Type", Name = "IX_Document_Type")]
public partial class Document
{
    [Key]
    [Column(TypeName = "BIGINT")]
    public long Id { get; set; }

    public string Type { get; set; } = null!;

    public string? Content { get; set; }

    [Column(TypeName = "BIGINT")]
    public long Version { get; set; }
}
