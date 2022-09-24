using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction
{
    public interface IComponentRegistration
    {
        void Component<TComponent>()
            where TComponent : class;

        void ValueType<TValueType>()
            where TValueType : class;

        void Package(IComponentPackage package, params string[] childNamespace);
    }
}
