using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class JobComment
    {
        [Key] public Guid Id { get; set; }
        [Required] public Guid JobId { get; set; }
        public Job Job { get; set; } = default!;


        [Required, MaxLength(2000)] public string Text { get; set; } = default!;
        [MaxLength(100)] public string? Author { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
