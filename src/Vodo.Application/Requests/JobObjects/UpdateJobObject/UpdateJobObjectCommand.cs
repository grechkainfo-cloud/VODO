using MediatR;
using NetTopologySuite.Geometries;
using Vodo.Models;

namespace Vodo.Application.Requests.JobObjects.UpdateJobObject
{
    public class UpdateJobObjectCommand : IRequest<System.Guid>
    {
        public System.Guid Id { get; set; }

        public string? Name { get; set; }

        public Point? Location { get; set; }

        public Address? Address { get; set; }

        public string? OwnerDivision { get; set; }

        public System.Guid? DivisionId { get; set; }

        // GeoJSON как строка, например {"type":"Point","coordinates":[10.5,20.5]}
        public string? PointGeoJson { get; set; }
    }
}
