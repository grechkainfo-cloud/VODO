using MediatR;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.JobObjects.CreateJobObject
{
    public class CreateJobObjectCommandHandler : IRequestHandler<CreateJobObjectCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public CreateJobObjectCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(CreateJobObjectCommand request, CancellationToken cancellationToken)
        {
            Point? point = null;
            if (!string.IsNullOrWhiteSpace(request.PointGeoJson))
            {
                var reader = new GeoJsonReader();
                point = reader.Read<Point>(request.PointGeoJson);
                if (point != null)
                {
                    point.SRID = 4326;                    
                }
            }

            var jobObject = new JobObject
            {
                Name = request.Name,
                Location = point,
                Address = request.Address,
                OwnerDivision = request.OwnerDivision,
                DivisionId = request.DivisionId
            };

            await _context.JobObjects.AddAsync(jobObject, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return jobObject.Id;
        }
    }
}
