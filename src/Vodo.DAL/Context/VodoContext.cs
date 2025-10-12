using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vodo.DAL.Context
{
    public class VodoContext : DbContext
    {
        public VodoContext(DbContextOptions<VodoContext> options) : base(options) { }
    }
}
