using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.Divisions.UpdateDivision
{
    public class UpdateDivisionCommandHandler : IRequestHandler<UpdateDivisionCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public UpdateDivisionCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            var division = await _context.Divisions
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (division == null)
                throw new KeyNotFoundException($"Division with Id {request.Id} not found");

            if (request.Name is not null)
                division.Name = request.Name;

            if (request.GeometryGeoJson is not null)
            {
                Geometry? geometry = null;
                if (!string.IsNullOrWhiteSpace(request.GeometryGeoJson))
                {
                    var reader = new GeoJsonReader();
                    geometry = reader.Read<Geometry>(request.GeometryGeoJson);
                    if (geometry != null)
                    {
                        geometry.SRID = 4326;
                        division.Geometry = geometry;
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return division.Id;
        }
    }
}