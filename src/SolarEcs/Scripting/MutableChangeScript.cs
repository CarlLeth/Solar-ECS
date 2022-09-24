using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Scripting
{
    public class MutableChangeScript<TModel>
    {
        public IDictionary<Guid, TModel> Assignments { get; }
        public ICollection<Guid> Unassignments { get; }

        public MutableChangeScript()
        {
            this.Assignments = new Dictionary<Guid, TModel>();
            this.Unassignments = new HashSet<Guid>();
        }

        public ChangeScript<TModel> ToChangeScript()
        {
            return new ChangeScript<TModel>(Assignments.ToDictionary(o => o.Key, o => o.Value), Unassignments.ToList());
        }

        public void Assign(Guid id, TModel model)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to assign a model to the empty id '{0}'", default(Guid)));
            }

            Unassignments.Remove(id);
            Assignments.Add(id, model);
        }

        public void Unassign(Guid id)
        {
            if (id == default(Guid))
            {
                throw new ArgumentException(string.Format("Attempted to unassign a model from the empty id '{0}'", default(Guid)));
            }

            Assignments.Remove(id);
            Unassignments.Add(id);
        }
    }
}
