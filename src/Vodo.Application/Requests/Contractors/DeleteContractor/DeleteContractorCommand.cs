using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Application.Requests.Contractors.DeleteContractor
{
    public class DeleteContractorCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
