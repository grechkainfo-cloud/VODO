using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Divisions.GetDivisions
{
    public class GetDivisionsQueryHandler : IRequestHandler<GetDivisionsQuery, IEnumerable<Division>>
    {
        private readonly VodoContext _context;

        public GetDivisionsQueryHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Division>> Handle(GetDivisionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Divisions.ToListAsync(cancellationToken);
        }
    }
}