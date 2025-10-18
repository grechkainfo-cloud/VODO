using System;
using MediatR;

namespace Vodo.Application.Requests.Jobs.DeleteJob
{
    public class DeleteJobCommand : IRequest
    {
        public System.Guid Id { get; set; }
    }
}
