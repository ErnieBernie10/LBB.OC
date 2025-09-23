using System;
using System.Collections.Generic;

namespace LBB.OC.Outbox.DataModels;

public partial class Outbox
{
    public int Id { get; set; }

    public string AggregateType { get; set; } = null!;

    public string AggregateId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Module { get; set; } = null!;

    public string Payload { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public int? RetryCount { get; set; }
}
