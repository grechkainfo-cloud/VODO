using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Divisions.CreateDivision
{
    public class CreateDivisionCommandHandler : IRequestHandler<CreateDivisionCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public CreateDivisionCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            Geometry? geometry = null;
            if (!string.IsNullOrWhiteSpace(request.GeometryGeoJson))
            {
                var reader = new GeoJsonReader();
                geometry = reader.Read<Geometry>(request.GeometryGeoJson);
                if (geometry != null) geometry.SRID = 4326;
            }

            var division = new Division
            {
                Name = request.Name,
                Geometry = geometry
            };

            await _context.Divisions.AddAsync(division, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return division.Id;
        }
    }
}