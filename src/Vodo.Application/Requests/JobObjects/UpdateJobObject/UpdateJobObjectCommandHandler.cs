using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.JobObjects.UpdateJobObject
{
    public class UpdateJobObjectCommandHandler : IRequestHandler<UpdateJobObjectCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public UpdateJobObjectCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(UpdateJobObjectCommand request, CancellationToken cancellationToken)
        {
            var jobObject = await _context.JobObjects
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (jobObject == null)
                throw new KeyNotFoundException($"JobObject with Id {request.Id} not found");

            if (request.Name is not null)
                jobObject.Name = request.Name;

            if (request.Location is not null)
            {
                Point? point = null;
                if (!string.IsNullOrWhiteSpace(request.PointGeoJson))
                {
                    var reader = new GeoJsonReader();
                    point = reader.Read<Point>(request.PointGeoJson);
                    if (point != null)
                    {
                        point.SRID = 4326;

                        jobObject.Location = point;
                    }
                }
            }

            if (request.Address is not null)
                jobObject.Address = request.Address;

            if (request.OwnerDivision is not null)
                jobObject.OwnerDivision = request.OwnerDivision;

            if (request.DivisionId is not null)
                jobObject.DivisionId = request.DivisionId;

            await _context.SaveChangesAsync(cancellationToken);

            return jobObject.Id;
        }
    }
}
