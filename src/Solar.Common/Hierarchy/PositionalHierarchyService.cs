using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Hierarchy
{
    public class PositionalHierarchyService : IHierarchyService
    {
        private IQueryPlan<HierarchyPosition> PositionQuery { get; set; }

        public PositionalHierarchyService(IQueryPlan<HierarchyPosition> positionQuery)
        {
            this.PositionQuery = positionQuery;
        }

        public HierarchyModel ConstructHierarchy(Guid hierarchy)
        {
            var positionGroups = PositionQuery
                .Where(o => o.Model.Hierarchy == hierarchy)
                .ExecuteAll()
                .Models()
                .GroupBy(o => o.Parent)
                .ToList();

            return CreateRecursive(hierarchy, 0, positionGroups);
        }

        public IQueryable<Guid> GetChildEntities(Guid hierarchy, Guid parentEntity)
        {
            return PositionQuery
                .Where(o => o.Model.Hierarchy == hierarchy && o.Model.Parent == parentEntity)
                .OrderBy(o => o.Model.Ordinal)
                .ImmaterialQuery
                .Select(o => o.Key);
        }
        
        private HierarchyModel CreateRecursive(Guid entity, double ordinal, IEnumerable<IGrouping<Guid, HierarchyPosition>> positionGroups)
        {
            var childEntities = positionGroups.Where(o => o.Key == entity).FirstOrDefault();
            var childModels = childEntities == null ? Enumerable.Empty<HierarchyModel>() : childEntities.OrderBy(o => o.Ordinal).Select(o => CreateRecursive(o.Entity, o.Ordinal, positionGroups));

            return new HierarchyModel(entity, ordinal, childModels.ToList());
        }

        public HierarchyNode<TModel> ArrangeIntoHierarchy<TModel>(IQueryPlan<TModel> query, Guid hierarchy)
        {
            var positions = PositionQuery.Where(o => o.Model.Hierarchy == hierarchy);

            var models = query.Join(positions, model => model.Key, pos => pos.Model.Entity,
                    (model, pos) => new UnarrangedNode<TModel>(pos.Model.Parent, model.Key, model.Model, pos.Model.Ordinal))
                .ExecuteAll()
                .Models()
                .ToList();

            var positionGroupsByParent = models.GroupBy(o => o.Parent).ToDictionary(o => o.Key);

            return CreateNodeRecursive(hierarchy, default(TModel), 0, positionGroupsByParent);
        }

        private HierarchyNode<TModel> CreateNodeRecursive<TModel>(Guid entity, TModel model, double ordinal, IDictionary<Guid, IGrouping<Guid, UnarrangedNode<TModel>>> positionGroupsByParent)
        {
            IEnumerable<HierarchyNode<TModel>> childModels;

            if (positionGroupsByParent.ContainsKey(entity))
            {
                var childEntities = positionGroupsByParent[entity];
                childModels = childEntities.OrderBy(o => o.Ordinal).Select(o => CreateNodeRecursive(o.Entity, o.Model, o.Ordinal, positionGroupsByParent)).ToList();
            }
            else
            {
                childModels = Enumerable.Empty<HierarchyNode<TModel>>();
            }

            return new HierarchyNode<TModel>(entity, model, ordinal, childModels);
        }

        private class UnarrangedNode<TModel>
        {
            public Guid Parent { get; private set; }
            public Guid Entity { get; private set; }
            public TModel Model { get; private set; }
            public double Ordinal { get; private set; }

            public UnarrangedNode(Guid parent, Guid entity, TModel model, double ordinal)
            {
                this.Parent = parent;
                this.Entity = entity;
                this.Model = model;
                this.Ordinal = ordinal;
            }

            private UnarrangedNode() { }
        }
    }
}
