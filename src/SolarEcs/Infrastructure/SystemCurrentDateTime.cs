using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Infrastructure
{
    public class SystemCurrentDateTime : ICurrentDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
