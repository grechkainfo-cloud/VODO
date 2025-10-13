using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class JobStream
    {
        [Key] public Guid Id { get; set; }


        public Guid? JobObjectId { get; set; }
        public JobObject? JobObject { get; set; }


        public Guid? JobId { get; set; }
        public Job? Job { get; set; }


        [Required] public StreamKind Kind { get; set; }


        [Required, MaxLength(500)] public string SourceUrl { get; set; } = default!; // Original RTSP/HLS source
        [MaxLength(500)] public string? PublicUrl { get; set; } // Public-access endpoint (signed if needed)


        public bool IsActive { get; set; } = true; // Flag for availability
        public int? LatencyMs { get; set; } // Stream delay metric


        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
