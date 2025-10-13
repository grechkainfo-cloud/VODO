using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vodo.Models
{
    public class EventLog
    {
        [Key] public Guid Id { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        [MaxLength(100)] public string Action { get; set; } = default!; // e.g. Job.StatusChanged
        [MaxLength(50)] public string Entity { get; set; } = default!; // Job, Site, Stream
        public Guid? EntityId { get; set; }
        [MaxLength(100)] public string? User { get; set; }
        [Column(TypeName = "jsonb")] public Dictionary<string, object>? Payload { get; set; }
    }
}
