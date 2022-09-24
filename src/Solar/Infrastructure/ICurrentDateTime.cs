using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Infrastructure
{
    public interface ICurrentDateTime
    {
        DateTime UtcNow { get; }
    }
}
