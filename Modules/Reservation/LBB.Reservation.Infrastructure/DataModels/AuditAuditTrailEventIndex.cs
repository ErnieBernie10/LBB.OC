using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("Audit_AuditTrailEventIndex")]
[Index("DocumentId", "EventId", "Category", "Name", "CorrelationId", "UserId", "NormalizedUserName", "CreatedUtc", Name = "IDX_AuditTrailEventIndex_DocumentId")]
[Index("DocumentId", Name = "IDX_FK_Audit_AuditTrailEventIndex")]
public partial class AuditAuditTrailEventIndex
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "BIGINT")]
    public long? DocumentId { get; set; }

    public string? EventId { get; set; }

    public string? Category { get; set; }

    public string? Name { get; set; }

    public string? CorrelationId { get; set; }

    public string? UserId { get; set; }

    public string? NormalizedUserName { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime? CreatedUtc { get; set; }
}
