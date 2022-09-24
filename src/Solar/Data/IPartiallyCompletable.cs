using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solar.Ecs.Data;

namespace Solar.Ecs.Data
{
    /// <summary>
    /// Represents data that can be stored in an incomplete state, and can be checked for completeness at runtime.
    /// </summary>
    public interface IPartiallyCompletable
    {
        bool IsComplete();
    }
}

namespace Solar
{
    public static class PartiallyCompletableExtensions
    {
        public static void EnsureComplete(this IPartiallyCompletable partiallyCompletable, string errorMessage = null)
        {
            if (partiallyCompletable == null)
            {
                throw new ArgumentNullException();
            }
            
            if (!partiallyCompletable.IsComplete())
            {
                errorMessage = errorMessage ?? "Argument is not complete.";
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
