using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Infrastructure
{
    public interface ICurrentDateTime
    {
        DateTime UtcNow { get; }
    }
}
