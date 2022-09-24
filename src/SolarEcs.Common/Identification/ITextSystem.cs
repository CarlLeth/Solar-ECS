using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Identification
{
    public interface ITextSystem : ISystem
    {
        IQueryPlan<TextModel> Query { get; }
        IRecipe<TextModel> Recipe { get; }
    }
}
