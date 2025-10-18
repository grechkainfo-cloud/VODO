using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Vodo.Models
{
    public class Division
    {
        [Key] public Guid Id { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = default!;

        public Geometry? Geometry { get; set; }
    }
}
