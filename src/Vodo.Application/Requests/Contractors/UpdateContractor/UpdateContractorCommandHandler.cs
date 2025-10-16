using MediatR;
using Microsoft.EntityFrameworkCore;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.Contractors.UpdateContractor
{
    public class UpdateContractorCommandHandler : IRequestHandler<UpdateContractorCommand, Guid>
    {
        private readonly VodoContext _context;

        public UpdateContractorCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(UpdateContractorCommand request, CancellationToken cancellationToken)
        {
            var contractor = await _context.Contractors
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (contractor == null)
                throw new Exception($"Contractor with Id {request.Id} not found");

            if (request.Name is not null)
                contractor.Name = request.Name;

            if (request.Inn is not null)
                contractor.Inn = request.Inn;

            if (request.Payload is not null)
                contractor.Payload = request.Payload;

            await _context.SaveChangesAsync(cancellationToken);

            return contractor.Id;
        }
    }
}
