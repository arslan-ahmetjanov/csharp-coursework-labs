using System.Text.Json.Serialization;

namespace RequestRegistry.Models;

public sealed class Request
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Author { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public RequestStatus Status { get; set; } = RequestStatus.New;

    public DateTime? ClosedAt { get; set; }

    [JsonIgnore]
    public double? ResolutionDays =>
        ClosedAt is null ? null : (ClosedAt.Value - CreatedAt).TotalDays;
}
