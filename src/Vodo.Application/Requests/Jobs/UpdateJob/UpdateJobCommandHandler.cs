using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vodo.DAL.Context;
using Vodo.Models;

namespace Vodo.Application.Requests.Jobs.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, System.Guid>
    {
        private readonly VodoContext _context;

        public UpdateJobCommandHandler(VodoContext context)
        {
            _context = context;
        }

        public async Task<System.Guid> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _context.Jobs
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (job == null)
                throw new KeyNotFoundException($"Job with Id {request.Id} not found");

            if (request.Title is not null)
                job.Title = request.Title;

            if (request.Description is not null)
                job.Description = request.Description;

            if (request.Type is not null)
                job.Type = request.Type.Value;

            if (request.StatusId is not null)
                job.StatusId = request.StatusId.Value;

            if (request.Priority is not null)
                job.Priority = request.Priority.Value;

            if (request.ContractorId is not null)
                job.ContractorId = request.ContractorId;

            if (request.JobObjectId is not null)
                job.JobObjectId = request.JobObjectId.Value;

            if (request.Geometry is not null)
                job.Geometry = request.Geometry;

            if (request.DaysOverdue is not null)
                job.DaysOverdue = request.DaysOverdue;

            await _context.SaveChangesAsync(cancellationToken);

            return job.Id;
        }
    }
}
