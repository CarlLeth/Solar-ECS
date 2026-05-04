using SolarEcs.Common.ChangeTracking;
using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public interface IVersioningSystem
    {
        IQueryPlan<T> LatestQuery<T>(IQueryPlan<T> query);
        IRecipe<T> LatestRecipe<T>(IRecipe<T> recipe, int? maxRetainedVersions = null);
        IWritePlan<T> LatestWritePlan<T>(IWritePlan<T> writePlan, int? maxRetainedVersions = null);

        IQueryPlan<WithVersions<T>> AttachVersions<T>(IQueryPlan<T> query);
    }

    public class VersioningSystem : IVersioningSystem
    {
        readonly IStore<EntityVersion> EntityVersions;
        readonly IDataAgent CurrentAgent;

        public VersioningSystem(IStore<EntityVersion> entityVersions, IDataAgent currentAgent)
        {
            EntityVersions = entityVersions;
            CurrentAgent = currentAgent;
        }

        public IQueryPlan<T> LatestQuery<T>(IQueryPlan<T> query)
        {
            // Filter the query to records either containing no version information, or
            // those whose ID is the same as the PrimaryEntity 
            return query.EntityLeftJoin(EntityVersions.ToQueryPlan())
                .Where((model, vers) => !vers.ValueOrEmpty.Any() || vers.Get(o => o.PrimaryEntity == model.Key && !o.IsDeleted, false))
                .ResolveJoin((model, vers) => model.Model);
        }

        public IRecipe<T> LatestRecipe<T>(IRecipe<T> recipe, int? maxRetainedVersions = null)
        {
            return new VersioningRecipe<T>(recipe, EntityVersions, this, CurrentAgent, maxRetainedVersions);
        }

        public IWritePlan<T> LatestWritePlan<T>(IWritePlan<T> writePlan, int? maxRetainedVersions = null)
        {
            var existingModels = LatestQuery(writePlan.ExistingModels);

            return WritePlan.Create(existingModels, script =>
            {
                var modelTrans = new MutableChangeScript<T>();
                var versionTrans = new MutableChangeScript<EntityVersion>();

                // The latest version record has Key == PrimaryEntity, so we can just filter on Key in the script.
                var latestModelVersions = writePlan.ExistingModels
                    .EntityLeftJoin(EntityVersions.ToQueryPlan())
                    .ResolveJoin((model, version) => new { model.Model, Version = version })
                    .Where(o => script.AllKeys.Contains(o.Key))
                    .ExecuteToDictionary();

                foreach (var assign in script.Assign)
                {
                    modelTrans.Assign(assign.Key, assign.Value);

                    if (latestModelVersions.ContainsKey(assign.Key))
                    {
                        // Existing model. Advance the version.
                        advanceVersions(assign.Key, isDeleting: false);
                    }
                    else
                    {
                        // New model. Create a version record with the same ID.
                        versionTrans.Assign(assign.Key, new EntityVersion(assign.Key, 1, DateTime.UtcNow, CurrentAgent.Id, false));
                    }
                }

                foreach (var unassignKey in script.Unassign)
                {
                    if (latestModelVersions.ContainsKey(unassignKey))
                    {
                        advanceVersions(unassignKey, isDeleting: true);
                        modelTrans.Unassign(unassignKey);
                    }
                }

                if (maxRetainedVersions.HasValue)
                {
                    RemoveExpired();
                }

                return Enumerable.Concat(
                    writePlan.Apply(modelTrans.ToChangeScript()),
                    EntityVersions.ToWritePlan().Apply(versionTrans.ToChangeScript())
                );

                void advanceVersions(Guid id, bool isDeleting)
                {
                    // We will create new records containing the current model data and current version info.
                    // This is the new ID that will be shared by the two.
                    var currentVersionNewID = Guid.NewGuid();

                    var currentVersion = latestModelVersions[id].Version.Get(o => o, null);
                    if (currentVersion == null)
                    {
                        // No version information has been saved for this model yet. Create the current version with unknown author and time.
                        currentVersion = new EntityVersion(id, 1, null, null, false);
                    }

                    // Save a snapshot of the existing model and version info with this separate ID. The primary model will keep its same ID as it changes.
                    modelTrans.Assign(currentVersionNewID, latestModelVersions[id].Model);
                    versionTrans.Assign(currentVersionNewID, currentVersion);

                    // Update the version info of the latest model.
                    versionTrans.Assign(id, new EntityVersion(id, currentVersion.VersionNumber + 1, DateTime.UtcNow, CurrentAgent.Id, isDeleting));
                }
                
                void RemoveExpired()
                {
                    var expiredKeys = queryExpiredKeys();

                    foreach (var expiredKey in expiredKeys)
                    {
                        modelTrans.Unassign(expiredKey);
                        versionTrans.Unassign(expiredKey);
                    }
                }

                IEnumerable<Guid> queryExpiredKeys()
                {
                    // Add 1 to max retained versions, since the stored VersionNumbers will be 1 behind the versions we are currently creating.
                    var maxVersionsQuery = EntityVersions.ToQueryPlan()
                        .GroupBy(o => o.PrimaryEntity)
                        .Select(grp => grp.Max(o => o.VersionNumber) - maxRetainedVersions.Value + 1);

                    return EntityVersions.ToQueryPlan()
                        .Join(maxVersionsQuery, vers => vers.Model.PrimaryEntity, maxVers => maxVers.Key)
                        .Where((vers, maxVers) => vers.Model.VersionNumber < maxVers.Model)
                        .TupledPlan
                        .ExecuteKeysOnly();
                }

            });

        }

        public IQueryPlan<WithVersions<T>> AttachVersions<T>(IQueryPlan<T> query)
        {
            return LatestQuery(query)
                .GroupJoin(EntityVersions.ToQueryPlan(), model => model.Key, vers => vers.Model.PrimaryEntity)
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
