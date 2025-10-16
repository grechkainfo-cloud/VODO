using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vodo.Models;

namespace Vodo.Application.Requests.Contractors.GetContractors
{
    public class GetContractorsQueryHandler : IRequestHandler<GetContractorsQuery, IEnumerable<Contractor>>
    {
        public Task<IEnumerable<Contractor>> Handle(GetContractorsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
