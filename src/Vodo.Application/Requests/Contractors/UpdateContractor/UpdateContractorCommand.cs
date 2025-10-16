using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Application.Requests.Contractors.UpdateContractor
{
    public class UpdateContractorCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Inn { get; set; }
        public Dictionary<string, object>? Payload { get; set; }
    }
}
