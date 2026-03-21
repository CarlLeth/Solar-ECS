using SolarEcs.Common.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public interface IVersioningSystem
    {
        IQueryPlan<T> LatestQuery<T>(IQueryPlan<T> query);
        IRecipe<T> LatestRecipe<T>(IRecipe<T> recipe);

        IQueryPlan<WithVersions<T>> AttachVersions<T>(IQueryPlan<T> query);
    }

    public class VersioningSystem : IVersioningSystem
    {
        readonly IStore<EntityVersion> Versions;
        readonly IDataAgent CurrentAgent;

        public VersioningSystem(IStore<EntityVersion> versions, IDataAgent currentAgent)
        {
            Versions = versions;
            CurrentAgent = currentAgent;
        }

        public IQueryPlan<T> LatestQuery<T>(IQueryPlan<T> query)
        {
            // Filter the query to records either containing no version information, or
            // those whose ID is the same as the PrimaryEntity 
            return query.EntityLeftJoin(Versions.ToQueryPlan())
                .Where((model, vers) => !vers.ValueOrEmpty.Any() || vers.Get(o => o.PrimaryEntity == model.Key && !o.IsDeleted, false))
                .ResolveJoin((model, vers) => model.Model);
        }

        public IRecipe<T> LatestRecipe<T>(IRecipe<T> recipe)
        {
            return new VersioningRecipe<T>(recipe, Versions, this, CurrentAgent);
        }

        public IQueryPlan<WithVersions<T>> AttachVersions<T>(IQueryPlan<T> query)
        {
            return LatestQuery(query)
                .GroupJoin(Versions.ToQueryPlan(), model => model.Key, vers => vers.Model.PrimaryEntity)
                .ResolveJoin((model, versions) => new WithVersions<T>(
                    model.Model,
                    versions
                        .Select(o => o.Model)
                        .Select(o => new EntityVersionStub(o.VersionNumber, o.VersionDate, o.ModifyingAgent, o.IsDeleted))
                        .OrderByDescending(o => o.VersionNumber)
                ));
        }
    }
}
