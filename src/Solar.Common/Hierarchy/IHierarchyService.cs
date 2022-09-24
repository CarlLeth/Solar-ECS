using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Hierarchy
{
    public interface IHierarchyService
    {
        HierarchyModel ConstructHierarchy(Guid hierarchy);
        IQueryable<Guid> GetChildEntities(Guid hierarchy, Guid parentEntity);
        HierarchyNode<TModel> ArrangeIntoHierarchy<TModel>(IQueryPlan<TModel> query, Guid hierarchy);
    }
}
