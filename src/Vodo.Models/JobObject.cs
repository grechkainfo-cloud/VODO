using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Vodo.Models
{
    public class JobObject
    {
        public Guid Id { get; set; }


        [Required, MaxLength(200)] public string Name { get; set; } = default!; // Name of the site


        public Point? Location { get; set; } // Coordinates (SRID 4326)


        public Address? Address { get; set; } // Full address


        [MaxLength(100)] public string? OwnerDivision { get; set; } // Responsible division or department


        // Navigation properties
        public List<Job> Jobs { get; set; } = new(); // All jobs related to the site
        public List<JobStream> Streams { get; set; } = new(); // Active video streams


        public Division? Division { get; set; }

        public Guid? DivisionId { get; set;  }

        [ConcurrencyCheck]
        public byte[]? RowVersion { get; set; }


        // Audit fields
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
