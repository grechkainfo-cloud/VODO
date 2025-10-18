using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vodo.DAL.Context;

namespace Vodo.Application.Requests.JobObjects.DeleteJobObject
{
    public class DeleteJobObjectCommandHandler : IRequestHandler<DeleteJobObjectCommand>
    {
        private readonly VodoContext _context;

        public DeleteJobObjectCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteJobObjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.JobObjects.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("JobObject not found");

            _context.JobObjects.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
