using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Vodo.DAL.Context;
using Vodo.Models;
using Microsoft.EntityFrameworkCore;

namespace Vodo.Application.Requests.Jobs.DeleteJob
{
    public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand>
    {
        private readonly VodoContext _context;

        public DeleteJobCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _context.Jobs.FindAsync(new object[] { request.Id }, cancellationToken);

            if (job == null)
                throw new KeyNotFoundException("Job not found");

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
