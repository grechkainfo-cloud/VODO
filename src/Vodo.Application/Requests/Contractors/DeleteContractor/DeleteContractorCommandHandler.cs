using MediatR;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.Contractors.DeleteContractor
{
    public class DeleteContractorCommandHandler : IRequestHandler<DeleteContractorCommand>
    {
        private readonly VodoContext _context;

        public DeleteContractorCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteContractorCommand request, CancellationToken cancellationToken)
        {
            var contractor = await _context.Contractors
                .FindAsync(request.Id, cancellationToken);

            if (contractor == null)
            {
                throw new KeyNotFoundException("Contractor not found");
            }

            _context.Contractors.Remove(contractor);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
