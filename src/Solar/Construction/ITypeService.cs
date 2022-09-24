using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction
{
    public interface ITypeService
    {
        IEnumerable<Type> GetImplementations(Type type);
        IEnumerable<ParameterInfo> GetConstructorParameters(Type type);

        bool IsConcreteType(Type type);
        bool IsEnumerableType(Type type);
        Type GetEnumeratedType(Type type);
    }
}
