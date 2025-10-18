using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.JobObjects.GetJobObjects
{
    public class GetJobObjectsQueryHandler : IRequestHandler<GetJobObjectsQuery, IEnumerable<JobObject>>
    {
        private readonly VodoContext _context;

        public GetJobObjectsQueryHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobObject>> Handle(GetJobObjectsQuery request, CancellationToken cancellationToken)
        {
            return await _context.JobObjects
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
