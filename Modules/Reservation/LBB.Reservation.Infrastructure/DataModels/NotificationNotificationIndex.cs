using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.DataModels;

[Table("Notification_NotificationIndex")]
[Index("DocumentId", Name = "IDX_FK_Notification_NotificationIndex")]
[Index("DocumentId", "NotificationId", "UserId", "IsRead", "CreatedAtUtc", "Content", Name = "IDX_NotificationIndex_DocumentId")]
[Index("DocumentId", "UserId", "IsRead", "CreatedAtUtc", Name = "IDX_NotificationIndex_UserId")]
public partial class NotificationNotificationIndex
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "BIGINT")]
    public long? DocumentId { get; set; }

    public string? NotificationId { get; set; }

    public string? UserId { get; set; }

    [Column(TypeName = "BOOL")]
    public bool? IsRead { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime? ReadAtUtc { get; set; }

    [Column(TypeName = "DATETIME")]
    public DateTime? CreatedAtUtc { get; set; }

    public string? Content { get; set; }
}
