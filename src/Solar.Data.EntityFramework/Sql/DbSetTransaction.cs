using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework.Sql
{
    public class DbSetTransaction<TPersisted, TComponent> : CachingComponentTransactionBase<TComponent>
        where TPersisted : EntityWith<TComponent>
        where TComponent : class
    {
        private IDbSet<TPersisted> DbSet;
        private Lazy<Func<TComponent, TPersisted>> ComponentToPersisted;
        private ICommitable Commitable;

        public DbSetTransaction(IDbSet<TPersisted> dbSet, IQueryPlan<TComponent> existingModels, Lazy<Func<TComponent, TPersisted>> componentToPersisted, ICommitable commitable)
            : base(existingModels)
        {
            ComponentToPersisted = componentToPersisted;
            DbSet = dbSet;
            Commitable = commitable;
        }

        protected override IEnumerable<ICommitable> Apply(IEnumerable<IKeyWith<Guid, TComponent>> assignments, IEnumerable<Guid> unassignments)
        {
            foreach (var assignment in assignments)
            {
                var existing = DbSet.Find(assignment.Key);
                if (existing != null)
                {
                    DbSet.Remove(existing);
                }

                var persisted = ComponentToPersisted.Value(assignment.Model);
                persisted.Id = assignment.Key;

                DbSet.Add(persisted);
            }

            foreach (var id in unassignments)
            {
                var existing = DbSet.Find(id);

                if (existing != null)
                {
                    DbSet.Remove(existing);
                }
            }

            yield return Commitable;
        }
    }
}
