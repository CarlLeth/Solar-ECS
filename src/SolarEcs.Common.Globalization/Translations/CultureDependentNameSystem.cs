using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Globalization.Translations
{
    public class CultureDependentNameSystem : INameSystem
    {
        private IStore<CultureDependentName> Names;
        private StaticNameSystem Fallback;
        private ITextCulture CurrentCulture;

        public CultureDependentNameSystem(IStore<CultureDependentName> names, StaticNameSystem fallback, ITextCulture currentCulture)
        {
            this.Names = names;
            this.Fallback = fallback;
            this.CurrentCulture = currentCulture;
        }

        public IQueryPlan<NameModel> Query
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Query;
                }

                var cultureSpecificNames = Names.ToQueryPlan()
                    .Where(o => o.Model.Culture == CurrentCulture.Id)
                    .ShiftKey(o => o.Model.Entity)
                    .Select(o => new NameModel(o.Name));

                return QueryPlan.UnionInPriorityOrder(
                    cultureSpecificNames,
                    Fallback.Query
                );
            }
        }


        public IRecipe<NameModel> Recipe
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.Recipe;
                }

                // TODO: There's a better way to provide translations than this
                return Query.StartRecipe()
                    .Include(Names.ToRecipe(),
                        assign: (trans, id, model) =>
                        {
                            var existing = trans.ExistingModels
                                .Where(o => o.Model.Entity == id && o.Model.Culture == CurrentCulture.Id)
                                .ExecuteKeysOnly();

                            var component = new CultureDependentName(id, CurrentCulture.Id, model.Name);
                            trans.Assign(existing.Any() ? existing.First() : Guid.NewGuid(), component);
                        },
                        unassign: (trans, id) => trans.UnassignWhere(o => o.Entity == id && o.Culture == CurrentCulture.Id)
                    );
            }
        }

        public IWritePlan<NameModel> WritePlan
        {
            get
            {
                if (!CurrentCulture.IsAvailable)
                {
                    return Fallback.WritePlan;
                }

                return Query.StartWritePlan()
                    .Include(Names.ToWritePlan(), (script, part, existing) =>
                    {
                        var existingNamesByEntity = existing
                            .Where(o => script.AllKeys.Contains(o.Model.Entity) && o.Model.Culture == CurrentCulture.Id)
                            .Select(o => o.Entity)
                            .ExecuteAll()
                            .ToDictionary(o => o.Model, o => o.Key);
                        
                        foreach (var name in script.Assign)
                        {
                            Guid cultureNameKey = existingNamesByEntity.ContainsKey(name.Key) ? existingNamesByEntity[name.Key] : Guid.NewGuid();
                            part.Assign(cultureNameKey, new CultureDependentName(name.Key, CurrentCulture.Id, name.Value.Name));
                        }

                        foreach (var key in script.Unassign)
                        {
                            if (existingNamesByEntity.ContainsKey(key))
                            {
                                part.Unassign(existingNamesByEntity[key]);
                            }
                        }
                    });
            }
        }
    }
}
