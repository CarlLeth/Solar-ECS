using SolarEcs.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Construction
{
    public class TypeService : ITypeService
    {
        private ConcurrentDictionary<Type, IEnumerable<Type>> FoundImplementations { get; set; }
        private ConcurrentDictionary<Type, IEnumerable<ParameterInfo>> ConstructorParameters { get; set; }

        public TypeService()
        {
            FoundImplementations = new ConcurrentDictionary<Type, IEnumerable<Type>>();
            ConstructorParameters = new ConcurrentDictionary<Type, IEnumerable<ParameterInfo>>();
        }

        public IEnumerable<Type> GetImplementations(Type type)
        {
            return FoundImplementations.GetOrAdd(type, o => FindImplementations(o));
        }

        private IEnumerable<Type> FindImplementations(Type abstractType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .SelectMany(assembly => FindImplementationsFromAssembly(abstractType, assembly))
                .ToList();
        }

        private IEnumerable<Type> FindImplementationsFromAssembly(Type abstractType, Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes
                    .Where(o => abstractType.IsAssignableFrom(o) && IsConcreteType(o) && o.IsPublic)
                    .Where(o => o.Namespace != "Castle.Proxies");
            }
            catch (ReflectionTypeLoadException)
            {
                return Enumerable.Empty<Type>();
            }
        }

        public bool IsConcreteType(Type type)
        {
            return !(type.IsInterface || type.IsAbstract);
        }

        public bool IsEnumerableType(Type type)
        {
            return type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }

        public Type GetEnumeratedType(Type type)
        {
            return type.GenericTypeArguments[0];
        }

        public IEnumerable<ParameterInfo> GetConstructorParameters(Type type)
        {
            return ConstructorParameters.GetOrAdd(type, o => FindConstructorParameters(o));
        }

        private IEnumerable<ParameterInfo> FindConstructorParameters(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            if (!constructors.Any())
            {
                throw new InvalidOperationException(string.Format("Type '{0}' must have a public constructor.", type));
            }

            var constructor = constructors.OrderByDescending(o => o.GetParameters().Length).FirstOrDefault();

            return constructor.GetParameters();
        }
    }
}
