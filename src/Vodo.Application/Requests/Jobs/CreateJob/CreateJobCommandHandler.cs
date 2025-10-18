using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Vodo.DAL.Context;
using Vodo.Models;
using Microsoft.EntityFrameworkCore;

namespace Vodo.Application.Requests.Jobs.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public CreateJobCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            Geometry? geometry = null;
            if (!string.IsNullOrWhiteSpace(request.GeometryGeoJson))
            {
                var reader = new GeoJsonReader();
                geometry = reader.Read<Geometry>(request.GeometryGeoJson);
                if (geometry != null)
                    geometry.SRID = 4326;
            }

            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
                StatusId = request.StatusId,
                Priority = request.Priority ?? JobPriority.Normal,
                ContractorId = request.ContractorId,
                JobObjectId = request.JobObjectId,
                Geometry = geometry
            };

            await _context.Jobs.AddAsync(job, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return job.Id;
        }
    }
}
