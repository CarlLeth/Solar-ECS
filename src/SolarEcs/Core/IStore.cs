using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SolarEcs.Infrastructure;

namespace SolarEcs
{
    /// <summary>
    /// Semantic interfacing indicating that this type should be treated like an IStore
    /// </summary>
    public interface IStore
    {
    }

    /// <summary>
    /// Provides data access to the component of type TComponent.
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    public interface IStore<TComponent> : IStore
    {
        ITransaction<TComponent> CreateTransaction();
        IQueryable<IEntityWith<TComponent>> All { get; }
        bool Contains(Guid id);
    }

    public static class IStoreExtensions
    {
        public static IQueryable<Guid> Entities<TComponent>(this IStore<TComponent> store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return store.All.Select(o => o.Id);
        }

        public static IQueryable<TComponent> Components<TComponent>(this IStore<TComponent> store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return store.All.Select(o => o.Component);
        }

        public static TComponent For<TComponent>(this IStore<TComponent> store, Guid id)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return store.All.Where(o => o.Id == id).Select(o => o.Component).FirstOrDefault();
        }

        public static Guid AddCommit<TComponent>(this IStore<TComponent> store, TComponent component)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (component == null) throw new ArgumentNullException(nameof(component));

            var trans = store.CreateTransaction();
            var entity = trans.Add(component);
            trans.Commit();
            return entity;
        }

        public static void AssignCommit<TComponent>(this IStore<TComponent> store, Guid id, TComponent component)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (component == null) throw new ArgumentNullException(nameof(component));

            var trans = store.CreateTransaction();
            trans.Assign(id, component);
            trans.Commit();
        }

        public static void AssignCommit<TComponent>(this IStore<TComponent> store, IEntity entity, TComponent component)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (component == null) throw new ArgumentNullException(nameof(component));

            store.AssignCommit(entity.Id, component);
        }

        public static void UnassignCommit<TComponent>(this IStore<TComponent> store, Guid id)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            var trans = store.CreateTransaction();
            trans.Unassign(id);
            trans.Commit();
        }

        public static void UnassignCommit<TComponent>(this IStore<TComponent> store, IEntity entity)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            store.UnassignCommit(entity.Id);
        }

        /// <summary>
        /// Immediately adds the given component to the store, unless an entity with a component matching uniqueConstraint already exists,
        /// in which case the existing component is overwritten.
        /// </summary>
        public static EntityWith<TComponent> AddUnique<TComponent>(this IStore<TComponent> store, TComponent component, Expression<Func<TComponent, bool>> uniqueKeyExpression)
        {
            var param = Expression.Parameter(typeof(IEntityWith<TComponent>));
            var componentAccessor = Expression.Property(param, "Component");

            var expressionVisitor = new ParameterReplacingExpressionVisitor();
            expressionVisitor.AddReplacementRule(uniqueKeyExpression.Parameters[0], componentAccessor);

            var fullKeyExpression = Expression.Lambda(expressionVisitor.Visit(uniqueKeyExpression.Body), param) as Expression<Func<IEntityWith<TComponent>, bool>>;

            var existing = store.All.SingleOrDefault(fullKeyExpression);

            Guid id;

            if (existing == null)
            {
                id = Guid.NewGuid();
            }
            else
            {
                id = existing.Id;
            }

            store.AssignCommit(id, component);

            return new EntityWith<TComponent>(id, component);
        }

        public static IQueryPlan<TComponent> ToQueryPlan<TComponent>(this IStore<TComponent> store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return new SolarEcs.Queries.EntityComponentQueryPlan<TComponent>(store);
        }

        public static IRecipe<TComponent> ToRecipe<TComponent>(this IStore<TComponent> store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return new SolarEcs.Recipes.ComponentRecipe<TComponent>(store);
        }
    }
}
