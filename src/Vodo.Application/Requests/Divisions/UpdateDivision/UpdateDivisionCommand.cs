using MediatR;
using NetTopologySuite.Geometries;

namespace Vodo.Application.Requests.Divisions.UpdateDivision
{
    public class UpdateDivisionCommand : IRequest<System.Guid>
    {
        public System.Guid Id { get; set; }

        public string? Name { get; set; }

        public string? GeometryGeoJson { get; set; }
    }
}