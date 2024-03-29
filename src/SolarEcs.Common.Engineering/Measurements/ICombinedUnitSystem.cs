﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public interface ICombinedUnitSystem
    {
        IQueryPlan<CombinedUnitModel> Query { get; }
        IRecipe<CombinedUnitModel> Recipe { get; }
    }
}
