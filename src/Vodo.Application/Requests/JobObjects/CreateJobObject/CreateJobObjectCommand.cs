using MediatR;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using Vodo.Models;

namespace Vodo.Application.Requests.JobObjects.CreateJobObject
{
    public class CreateJobObjectCommand : IRequest<System.Guid>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        // GeoJSON как строка, например {"type":"Point","coordinates":[10.5,20.5]}
        public string? PointGeoJson { get; set; }

        public Address? Address { get; set; }

        [MaxLength(100)]
        public string? OwnerDivision { get; set; }

        public System.Guid? DivisionId { get; set; }
    }
}
