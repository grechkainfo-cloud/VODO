using MediatR;

namespace Vodo.Application.Requests.Divisions.DeleteDivision
{
    public class DeleteDivisionCommand : IRequest
    {
        public System.Guid Id { get; set; }
    }
}