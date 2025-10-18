using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Contractors.GetContractors
{
    public class GetContractorsQueryHandler : IRequestHandler<GetContractorsQuery, IEnumerable<Contractor>>
    {
        private readonly VodoContext _context;

        public GetContractorsQueryHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contractor>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Contractors.ToListAsync(cancellationToken);
        }
    }
}
