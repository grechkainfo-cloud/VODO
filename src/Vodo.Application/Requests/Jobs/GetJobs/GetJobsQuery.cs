using MediatR;
using System.Collections.Generic;
using Vodo.Models;

namespace Vodo.Application.Requests.Jobs.GetJobs
{
    public class GetJobsQuery : IRequest<IEnumerable<Job>>
    {
    }
}
