using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction
{
    public interface IDependencyFactory<TDependency>
    {
        TDependency Build();
    }
}
