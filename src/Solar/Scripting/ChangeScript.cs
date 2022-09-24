using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Scripting
{
    public static class ChangeScript
    {
        /// <summary>
        /// Creates a new empty change script of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ChangeScript<T> Empty<T>()
        {
            return new ChangeScript<T>(new Dictionary<Guid, T>(), Enumerable.Empty<Guid>());
        }

        /// <summary>
        /// Creates a new change script of type T with the given assignments and unassignments.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assignments"></param>
        /// <param name="unassignments"></param>
        /// <returns></returns>
        public static ChangeScript<T> From<T>(IReadOnlyDictionary<Guid, T> assignments, IEnumerable<Guid> unassignments)
        {
            return new ChangeScript<T>(assignments, unassignments);
        }

        /// <summary>
        /// Creates a new change script of type T with only the given assignments (no unassignments).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public static ChangeScript<T> Assign<T>(IReadOnlyDictionary<Guid, T> assignments)
        {
            return new ChangeScript<T>(assignments, Enumerable.Empty<Guid>());
        }

        /// <summary>
        /// Creates a new change script of type T with only the given assignments, specified by a parameter list of key-model tuples.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public static ChangeScript<T> Assign<T>(params (Guid key, T model)[] assignments)
        {
            return Assign(assignments.ToDictionary(o => o.key, o => o.model));
        }

        /// <summary>
        /// Creates a new change script of type T with only the given unassignments (no assignments).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="unassignments"></param>
        /// <returns></returns>
        public static ChangeScript<T> Unassign<T>(IEnumerable<Guid> unassignments)
        {
            return new ChangeScript<T>(new Dictionary<Guid, T>(), unassignments);
        }

        public static ChangeScript<T> Unassign<T>(params Guid[] unassignments)
        {
            return Unassign<T>(unassignments.AsEnumerable());
        }
    }

    public class ChangeScript<TModel>
    {
        public IReadOnlyDictionary<Guid, TModel> Assign { get; }
        public IEnumerable<Guid> Unassign { get; }

        public ChangeScript(IReadOnlyDictionary<Guid, TModel> assign, IEnumerable<Guid> unassign)
        {
            this.Assign = assign;
            this.Unassign = unassign;
        }

        /// <summary>
        /// Returns a new ChangeScript whose elements are transformed by the given function.
        /// </summary>
        /// <typeparam name="TTransformed"></typeparam>
        /// <param name="transform"></param>
        /// <returns></returns>
        public ChangeScript<TTransformed> Select<TTransformed>(Func<TModel, TTransformed> transform)
        {
            return new ChangeScript<TTransformed>(
                assign: Assign.ToDictionary(o => o.Key, o => transform(o.Value)),
                unassign: Unassign
            );
        }

        /// <summary>
        /// Returns a new ChangeScript whose elements are transformed by the given function, taking each key into account.
        /// </summary>
        /// <typeparam name="TTransformed"></typeparam>
        /// <param name="script"></param>
        /// <param name="transformWithKey"></param>
        /// <returns></returns>
        public ChangeScript<TTransformed> Select<TTransformed>(Func<Guid, TModel, TTransformed> transformWithKey)
        {
            return new ChangeScript<TTransformed>(
                assign: Assign.ToDictionary(o => o.Key, o => transformWithKey(o.Key, o.Value)),
                unassign: Unassign
            );
        }

        /// <summary>
        /// Returns a new ChangeScript that represents the application of this script followed by the provided script.
        /// </summary>
        /// <param name="nextScript">The script to apply after this one.</param>
        /// <returns></returns>
        public ChangeScript<TModel> Then(ChangeScript<TModel> nextScript)
        {
            var assign = Assign.ToDictionary(o => o.Key, o => o.Value);
            var unassign = new HashSet<Guid>(Unassign);

            nextScript.Assign.ForEach(o =>
            {
                if (unassign.Contains(o.Key))
                {
                    unassign.Remove(o.Key);
                }

                assign[o.Key] = o.Value;
            });

            nextScript.Unassign.ForEach(o =>
            {
                if (assign.ContainsKey(o))
                {
                    assign.Remove(o);
                }

                if (!unassign.Contains(o))
                {
                    unassign.Add(o);
                }
            });

            return new ChangeScript<TModel>(assign, unassign);
        }

        public ChangeScript<TCast> Cast<TCast>()
        {
            return Select(o => (TCast)(object)o);
        }
    }
}
