using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Infrastructure
{
    public class SystemCurrentDateTime : ICurrentDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
