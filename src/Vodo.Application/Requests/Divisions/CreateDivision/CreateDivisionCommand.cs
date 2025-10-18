using MediatR;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Vodo.Application.Requests.Divisions.CreateDivision
{
    public class CreateDivisionCommand : IRequest<System.Guid>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public string? GeometryGeoJson { get; set; }
    }
}