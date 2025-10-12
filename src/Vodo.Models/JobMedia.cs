using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class JobMedia
    {
        [Key] public Guid Id { get; set; }
        [Required] public Guid JobId { get; set; }
        public Job Job { get; set; } = default!;


        [Required] public MediaKind Kind { get; set; }
        [Required, MaxLength(500)] public string Url { get; set; } = default!; // File location (S3/MinIO)
        public DateTimeOffset? TakenAt { get; set; } // Date/time when photo/video was taken


        [Column(TypeName = "jsonb")] public Dictionary<string, object>? Meta { get; set; } // Metadata (camera, GPS, etc.)
    }
}
