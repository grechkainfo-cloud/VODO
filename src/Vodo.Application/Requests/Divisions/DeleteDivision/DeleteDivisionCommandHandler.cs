using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.Divisions.DeleteDivision
{
    public class DeleteDivisionCommandHandler : IRequestHandler<DeleteDivisionCommand>
    {
        private readonly VodoContext _context;

        public DeleteDivisionCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            var division = await _context.Divisions.FindAsync(new object[] { request.Id }, cancellationToken);

            if (division == null)
                throw new KeyNotFoundException("Division not found");

            _context.Divisions.Remove(division);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}