using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs
{
    public interface IComponentWriteObserver
    {
        void OnComponentAssigned(Guid entity, object assignedComponent);
        void OnComponentUnassigned(Guid entity);
    }
}
