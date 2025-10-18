using System;
using MediatR;

namespace Vodo.Application.Requests.JobObjects.DeleteJobObject
{
    public class DeleteJobObjectCommand : IRequest
    {
        public System.Guid Id { get; set; }
    }
}
