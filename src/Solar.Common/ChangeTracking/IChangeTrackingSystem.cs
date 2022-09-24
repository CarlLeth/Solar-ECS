using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.ChangeTracking
{
    /// <summary>
    /// Tracks the date and user of every commit of arbitrary entities.
    /// Does not track the nature of the change, only the fact that a change was made.
    /// </summary>
    public interface IChangeTrackingSystem
    {
        IQueryPlan<WithChangeEvents<T>> AttachTo<T>(IQueryPlan<T> baseQuery);
        IRecipe<T> AttachTo<T>(IRecipe<T> baseRecipe, Guid? changingAgent = null);
    }
}
