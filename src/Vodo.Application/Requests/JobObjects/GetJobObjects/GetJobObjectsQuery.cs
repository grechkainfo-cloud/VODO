using MediatR;
using System.Collections.Generic;
using Vodo.Models;

namespace Vodo.Application.Requests.JobObjects.GetJobObjects
{
    public class GetJobObjectsQuery : IRequest<IEnumerable<JobObject>>
    {
    }
}
