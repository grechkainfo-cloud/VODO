using MediatR;
using System.Collections.Generic;
using Vodo.Models;

namespace Vodo.Application.Requests.Divisions.GetDivisions
{
    public class GetDivisionsQuery : IRequest<IEnumerable<Division>>
    {
    }
}