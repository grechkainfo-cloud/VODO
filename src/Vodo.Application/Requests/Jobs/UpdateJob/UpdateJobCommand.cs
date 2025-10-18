using MediatR;
using NetTopologySuite.Geometries;
using Vodo.Models;

namespace Vodo.Application.Requests.Jobs.UpdateJob
{
    public class UpdateJobCommand : IRequest<System.Guid>
    {
        public System.Guid Id { get; set; }

        // Partial update fields (null = не изменять)
        public string? Title { get; set; }
        public string? Description { get; set; }
        public JobType? Type { get; set; }
        public int? StatusId { get; set; }
        public JobPriority? Priority { get; set; }
        public System.Guid? ContractorId { get; set; }
        public System.Guid? JobObjectId { get; set; }
        public Geometry? Geometry { get; set; }
        public int? DaysOverdue { get; set; }
    }
}
