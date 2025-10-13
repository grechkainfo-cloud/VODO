using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vodo.Models
{
    public class Contractor
    {
        [Key] public Guid Id { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = default!;
        [MaxLength(12)] public string? Inn { get; set; } // Russian tax ID
        [Column(TypeName = "jsonb")] public Dictionary<string, object>? Payload { get; set; } // Email/Phone/Person, etc.
    }
}
