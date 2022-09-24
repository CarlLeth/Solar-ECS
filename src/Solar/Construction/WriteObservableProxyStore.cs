using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction
{
    public class WriteObservableProxyStore<TComponent> : IStore<TComponent>
    {
        private IStore<TComponent> BaseStore { get; set; }
        private IEnumerable<IComponentWriteObserver> WriteObservers { get; set; }

        public WriteObservableProxyStore(IStore<TComponent> baseStore, IEnumerable<IComponentWriteObserver> writeObservers)
        {
            this.BaseStore = baseStore;
            this.WriteObservers = writeObservers;
        }

        public ITransaction<TComponent> CreateTransaction()
        {
            return new WriteObservableProxyTransaction(BaseStore.CreateTransaction(), WriteObservers);
        }

        public IQueryable<IEntityWith<TComponent>> All
        {
            get
            {
                return BaseStore.All;
            }
        }

        public bool Contains(Guid id)
        {
            return BaseStore.Contains(id);
        }

        private class WriteObservableProxyTransaction : ITransaction<TComponent>
        {
            private ITransaction<TComponent> BaseTransaction { get; set; }
            private IEnumerable<IComponentWriteObserver> WriteObservers { get; set; }

            private IDictionary<Guid, object> Assignments { get; set; }
            private ISet<Guid> Unassignments { get; set; }

            public WriteObservableProxyTransaction(ITransaction<TComponent> baseTransaction, IEnumerable<IComponentWriteObserver> writeObservers)
            {
                this.BaseTransaction = baseTransaction;
                this.WriteObservers = writeObservers;

                this.Assignments = new Dictionary<Guid, object>();
                this.Unassignments = new HashSet<Guid>();
            }

            public void Assign(Guid id, TComponent component)
            {
                Assignments[id] = component;

                if (Unassignments.Contains(id))
                {
                    Unassignments.Remove(id);
                }

                BaseTransaction.Assign(id, component);
            }

            public void Unassign(Guid id)
            {
                if (!Unassignments.Contains(id))
                {
                    Unassignments.Add(id);
                }

                if (Assignments.ContainsKey(id))
                {
                    Assignments.Remove(id);
                }

                BaseTransaction.Unassign(id);
            }

            public IEnumerable<ICommitable> ApplyChanges()
            {
                var commitables = BaseTransaction.ApplyChanges();

                foreach (var writeObserver in WriteObservers)
                {
                    NotifyWrites(writeObserver);
                }

                return commitables;
            }

            private void NotifyWrites(IComponentWriteObserver writeObserver)
            {
                foreach (var assignment in Assignments)
                {
                    writeObserver.OnComponentAssigned(assignment.Key, assignment.Value);
                }

                foreach (var unassignment in Unassignments)
                {
                    writeObserver.OnComponentUnassigned(unassignment);
                }
            }

            public bool IsValid
            {
                get { return BaseTransaction.IsValid; }
            }

            public void Invalidate(string failureMessage = null)
            {
                BaseTransaction.Invalidate(failureMessage);
            }

            public bool CanAssign(Guid id, TComponent model)
            {
                return BaseTransaction.CanAssign(id, model);
            }

            public IQueryPlan<TComponent> ExistingModels
            {
                get
                {
                    return BaseTransaction.ExistingModels;
                }
            }
        }
    }
}
