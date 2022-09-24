using Solar.Ecs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.EntityManagement
{
    public enum ComponentTraversalMode
    {
        /// <summary>
        /// Only traverse components directly owned by the visited entity.
        /// </summary>
        TraverseOwnedComponentsOnly,

        /// <summary>
        /// Traverse components which have a relation to the visited entity.
        /// Much worse performance than traversing owned components only.
        /// </summary>
        TraverseRelations
    }

    public abstract class EntityVisitor
    {
        private static readonly MethodInfo VisitComponentIfExistsMethod = typeof(EntityVisitor).GetMethod("VisitComponentIfExists", BindingFlags.Instance | BindingFlags.NonPublic);

        private ComponentTraversalMode TraversalMode { get; set; }

        private Dictionary<Type, IStore> Stores { get; set; }

        protected EntityVisitor(IComponentCatalog catalog, ComponentTraversalMode traversalMode)
        {
            this.Stores = catalog.AvailableComponentTypes.ToDictionary(type => type, type => catalog.Store(type));
            this.TraversalMode = traversalMode;
        }

        public void VisitEntities(IEnumerable<Guid> entities)
        {
            entities.ForEach(entity => VisitEntity(entity));
        }

        public void VisitEntity(Guid entity)
        {
            foreach (var storePair in Stores.Where(o => ShouldVisitComponentType(o.Key, entity)))
            {
                VisitComponentIfExistsMethod
                    .MakeGenericMethod(storePair.Key)
                    .Invoke(this, new object[] { entity, storePair.Value });
            }
        }

        private void VisitComponentIfExists<TComponent>(Guid entity, IStore<TComponent> store)
        {
            VisitOwnedComponentIfExists(entity, store);

            if (TraversalMode == ComponentTraversalMode.TraverseRelations)
            {
                VisitRelatedComponentsIfAny(entity, store);
            }
        }

        private void VisitOwnedComponentIfExists<TComponent>(Guid entity, IStore<TComponent> store)
        {
            if (store.Contains(entity))
            {
                VisitComponent(entity, store.All.First(o => o.Id == entity).Component);
            }
        }

        private void VisitRelatedComponentsIfAny<TComponent>(Guid entity, IStore<TComponent> store)
        {
            var relationProperties = typeof(TComponent).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.PropertyType == typeof(Guid));

            foreach (var relationProperty in relationProperties)
            {
                var relatedEntities = store.All.Where(CreateRelationPredicate<TComponent>(entity, relationProperty))
                    .ToList()
                    .Cast<EntityWith<TComponent>>();

                if (relatedEntities.Any())
                {
                    VisitRelatedEntities(entity, relatedEntities, relationProperty);
                }
            }
        }

        private Expression<Func<IEntityWith<TComponent>, bool>> CreateRelationPredicate<TComponent>(Guid entity, PropertyInfo relation)
        {
            var param = Expression.Parameter(typeof(IEntityWith<TComponent>));
            var componentExpr = Expression.Property(param, "Component");
            var propertyExpr = Expression.Property(componentExpr, relation);

            var comparison = Expression.Equal(propertyExpr, Expression.Constant(entity));

            return (Expression<Func<IEntityWith<TComponent>, bool>>)Expression.Lambda(comparison, param);
        }

        protected virtual bool ShouldVisitComponentType(Type componentType, Guid entity)
        {
            return true;
        }

        protected virtual void VisitComponent<TComponent>(Guid entity, TComponent component)
        {
            //No default implementation
        }

        protected virtual void VisitRelatedEntities<TComponent>(Guid entity, IEnumerable<EntityWith<TComponent>> relatedEntities, PropertyInfo relation)
        {
            //No default implementation
        }
    }
}
