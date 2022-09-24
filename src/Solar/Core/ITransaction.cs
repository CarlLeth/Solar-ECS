using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Solar
{
    public interface ICommitable
    {
        void Commit();
    }

    public interface ITransaction
    {
        IEnumerable<ICommitable> ApplyChanges();

        bool IsValid { get; }
        void Invalidate();
    }

    public interface ITransaction<TComponent> : ITransaction
    {
        bool CanAssign(Guid id, TComponent model);
        void Assign(Guid id, TComponent component);
        void Unassign(Guid id);

        IQueryPlan<TComponent> ExistingModels { get; }
    }

    public static class ITransactionExtensions
    {
        public static void Commit(this ITransaction transaction)
        {
            var commitables = transaction.ApplyChanges();

            if (commitables.Count() <= 1)
            {
                commitables.ForEach(o => o.Commit());
            }
            else
            {
                using (var scope = new TransactionScope())
                {
                    commitables.ForEach(o => o.Commit());
                    scope.Complete();
                }
            }
        }

        public static Guid Add<TComponent>(this ITransaction<TComponent> store, TComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            var id = Guid.NewGuid();
            store.Assign(id, component);
            return id;
        }

        public static void UnassignWhere<TComponent>(this ITransaction<TComponent> transaction, Expression<Func<TComponent, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            Expression<Func<IKeyWith<Guid, TComponent>, TComponent>> modelSelector = o => o.Model;
            var fullPredicate = modelSelector.Into(predicate);

            var matches = transaction.ExistingModels.Where(fullPredicate).ExecuteAll();
            matches.ForEach(o => transaction.Unassign(o.Key));
        }

        public static void Unassign<TComponent>(this ITransaction<TComponent> store, IEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            store.Unassign(entity.Id);
        }

        public static void Assign<TComponent>(this ITransaction<TComponent> store, IEntity entity, TComponent component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            store.Assign(entity.Id, component);
        }
    }
}
