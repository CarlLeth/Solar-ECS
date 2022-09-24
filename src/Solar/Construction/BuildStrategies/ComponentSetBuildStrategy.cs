using Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction.BuildStrategies
{
    public sealed class ComponentSetBuildStrategy : IBuildStrategy
    {
        private static readonly MethodInfo BuildComponentSetGenericMethod = typeof(ComponentSetBuildStrategy).GetMethod("BuildComponentSetGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
        
        private IComponentCatalog Catalog { get; set; }
        private ITypeService TypeService { get; set; }

        public ComponentSetBuildStrategy(IComponentCatalog catalog, ITypeService typeService)
        {
            this.Catalog = catalog;
            this.TypeService = typeService;
        }

        public bool CanBuild(Type type)
        {
            if (TypeService.IsEnumerableType(type))
            {
                Type enumeratedType = TypeService.GetEnumeratedType(type);
                Type componentType = IsEntityWith(enumeratedType) ? enumeratedType.GetGenericArguments()[0] : enumeratedType;

                return Catalog.Contains(componentType);
            }
            else
            {
                return false;
            }
        }

        public BuildResult Build(Type type, IBuildSession buildSession)
        {
            return BuildResult.Success(() =>
            {
                Type enumeratedType = TypeService.GetEnumeratedType(type);
                bool isEntityWith = IsEntityWith(enumeratedType);

                //IQueryables will be kept immaterial; everything else will be materialized.
                bool isQueryable = type.GetGenericTypeDefinition() == typeof(IQueryable<>);

                Type componentType = isEntityWith ? enumeratedType.GetGenericArguments()[0] : enumeratedType;

                return BuildComponentSetGenericMethod.MakeGenericMethod(componentType).Invoke(this, new object[] { isEntityWith, isQueryable });
            });
        }

        private object BuildComponentSetGeneric<TComponent>(bool isEntityWith, bool isQueryable)
        {
            var query = Catalog.Store<TComponent>().All;

            if (isEntityWith && isQueryable)
            {
                return query;
            }
            else if (isEntityWith && !isQueryable)
            {
                return query.ToList();
            }
            else if (!isEntityWith && isQueryable)
            {
                return query.Select(o => o.Component);
            }
            else if (!isEntityWith && !isQueryable)
            {
                return query.Select(o => o.Component).ToList();
            }

            return Catalog.Store<TComponent>().All;
        }

        private bool IsEntityWith(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEntityWith<>);
        }
    }
}
