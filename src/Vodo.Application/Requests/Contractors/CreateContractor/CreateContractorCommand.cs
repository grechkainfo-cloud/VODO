using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Vodo.Application.Requests.Contractors.CreateContractor
{
    public class CreateContractorCommand : IRequest<Guid>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        [MaxLength(12)]
        public string? Inn { get; set; }

        public Dictionary<string, object>? Payload { get; set; }
    }
}
