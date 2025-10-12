using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class Job
    {
        [Key] public Guid Id { get; set; }


        // Foreign keys
        public Guid SiteId { get; set; }
        public JobObject JobObject { get; set; } = default!;


        [Required, MaxLength(200)] public string Title { get; set; } = default!; // Job title
        [MaxLength(2000)] public string? Description { get; set; } // Detailed description


        [Required] public JobType Type { get; set; } // Type of work
        [Required] public int StatusId { get; set; } // Foreign key to JobStatus
        public JobStatus Status { get; set; } = default!;


        [Required] public JobPriority Priority { get; set; } = JobPriority.Normal; // Priority level


        public Guid? ContractorId { get; set; } // Contractor performing job
        public Contractor? Contractor { get; set; }


        // Schedule information
        public PlannedActualPeriod PlannedFact { get; set; } = new();


        // Optional geometry (e.g. trench polygon or route)
        public Geometry? Geometry { get; set; }


        // Calculated fields
        public int? DaysOverdue { get; set; } // SLA calculation result


        // Related entities
        public List<JobMedia> Media { get; set; } = new(); // Photos, documents, etc.
        public List<JobComment> Comments { get; set; } = new(); // Internal notes
        public List<Stream> Streams { get; set; } = new(); // Associated video streams


        [Timestamp] public byte[]? RowVersion { get; set; }


        // Audit fields
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
