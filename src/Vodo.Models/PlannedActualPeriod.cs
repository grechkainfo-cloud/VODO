using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.Models
{
    public class PlannedActualPeriod
    {
        public DateTimeOffset? PlannedStart { get; set; }
        public DateTimeOffset? PlannedEnd { get; set; }
        public DateTimeOffset? FactStart { get; set; }
        public DateTimeOffset? FactEnd { get; set; }
    }
}
