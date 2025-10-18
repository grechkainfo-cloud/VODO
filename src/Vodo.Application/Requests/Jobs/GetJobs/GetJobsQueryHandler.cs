using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Jobs.GetJobs
{
    public class GetJobsQueryHandler : IRequestHandler<GetJobsQuery, IEnumerable<Job>>
    {
        private readonly VodoContext _context;

        public GetJobsQueryHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Job>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
        {
            // Возвращаем полный список работ. При необходимости можно добавить .Include(...) для навигационных свойств.
            return await _context.Jobs
                                 .AsNoTracking()
                                 .ToListAsync(cancellationToken);
        }
    }
}
