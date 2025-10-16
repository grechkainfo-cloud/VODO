using MediatR;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Contractors.CreateContractor
{
    public class CreateContractorCommandHandler : IRequestHandler<CreateContractorCommand, Guid>
    {
        private readonly VodoContext _context;

        public CreateContractorCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateContractorCommand request, CancellationToken cancellationToken)
        {
            var contractor = new Contractor
            {
                Name = request.Name,
                Inn = request.Inn,
                Payload = request.Payload
            };

            await _context.Contractors.AddAsync(contractor, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return contractor.Id;
        }
    }
}
