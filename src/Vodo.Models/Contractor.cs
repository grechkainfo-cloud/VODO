using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class Contractor
    {
        [Key] public Guid Id { get; set; }
        [Required, MaxLength(200)] public string Name { get; set; } = default!;
        [MaxLength(12)] public string? Inn { get; set; } // Russian tax ID
        [Column(TypeName = "jsonb")] public Dictionary<string, object>? Contacts { get; set; } // Email/Phone/Person, etc.
    }
}
