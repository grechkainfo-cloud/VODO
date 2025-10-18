using MediatR;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Vodo.Application.Requests.Jobs.CreateJob
{
    public class CreateJobCommand : IRequest<System.Guid>
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = default!;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public Vodo.Models.JobType Type { get; set; }

        [Required]
        public int StatusId { get; set; }

        public Vodo.Models.JobPriority? Priority { get; set; } = Vodo.Models.JobPriority.Normal;

        public System.Guid? ContractorId { get; set; }

        [Required]
        public System.Guid JobObjectId { get; set; }

        // GeoJSON строка, например {"type":"Point","coordinates":[10.5,20.5]}
        public string? GeometryGeoJson { get; set; }
    }
}
