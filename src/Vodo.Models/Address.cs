using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{    
    public class Address
    {
        [MaxLength(200)] public string? Line1 { get; set; }
        [MaxLength(200)] public string? Line2 { get; set; }
        [MaxLength(100)] public string? City { get; set; }
        [MaxLength(100)] public string? District { get; set; }
        [MaxLength(20)] public string? PostalCode { get; set; }
    }
}
