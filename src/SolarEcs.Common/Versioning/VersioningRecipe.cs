using SolarEcs.Common.ChangeTracking;
using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolarEcs.Common.Versioning
{
    public class VersioningRecipe<T> : IRecipe<T>
    {
        readonly IRecipe<T> ModelRecipe;
        readonly IStore<EntityVersion> EntityVersions;
        readonly VersioningSystem VersioningSystem;
        readonly IDataAgent CurrentAgent;
        readonly int? MaxRetainedVersions;

        public VersioningRecipe(IRecipe<T> modelRecipe, IStore<EntityVersion> entityVersions, VersioningSystem versioningSystem, IDataAgent currentAgent, int? maxRetainedVersions)
        {
            ModelRecipe = modelRecipe;
            EntityVersions = entityVersions;
            VersioningSystem = versioningSystem;
            CurrentAgent = currentAgent;
            MaxRetainedVersions = maxRetainedVersions;
        }

        public IQueryPlan<T> ExistingModels => VersioningSystem.LatestQuery(ModelRecipe.ExistingModels);

        public ITransaction<T> CreateTransaction()
        {
            var modelTrans = ModelRecipe.CreateTransaction();
            var versionTrans = EntityVersions.CreateTransaction();

            void advanceVersions(Guid id, T currentModel, bool isDeleting)
            {
                // We will create new records containing the current model data and current version info.
                // This is the new ID that will be shared by the two.
                var currentVersionNewID = Guid.NewGuid();

                var currentVersion = EntityVersions.For(id);

                if (currentVersion == null)
                {
                    // No version information has been saved for this model yet. Create the current version with unknown author and time.
                    currentVersion = new EntityVersion(id, 1, null, null, false);
                }
                else if (MaxRetainedVersions.HasValue)
                {
                    // Add 1, since currentVersion.VersionNumber is 1 version behind the version we are currently creating.
                    int maxVersionToKeep = currentVersion.VersionNumber - MaxRetainedVersions.Value + 1;

                    var expiredVersionKeys = EntityVersions.ToQueryPlan()
                        .Where(version => version.Model.PrimaryEntity == id && version.Model.VersionNumber < maxVersionToKeep)
                        .ExecuteKeysOnly();

                    foreach (var expiredKey in expiredVersionKeys)
                    {
                        modelTrans.Unassign(expiredKey);
                        versionTrans.Unassign(expiredKey);
                    }
                }

                modelTrans.Assign(currentVersionNewID, currentModel);
                versionTrans.Assign(currentVersionNewID, currentVersion);

                versionTrans.Assign(id, new EntityVersion(id, currentVersion.VersionNumber + 1, DateTime.UtcNow, CurrentAgent.Id, isDeleting));
            }

            return new EagerTransaction<T>(
                canAssignPredicate: (id, model) => modelTrans.CanAssign(id, model),
                assignAction: (id, model) =>
                {
                    var currentModel = modelTrans.ExistingModels.Execute(id);

                    if (currentModel == null)
                    {
                        // Entirely new model. Create a version record with the same ID.
                        modelTrans.Assign(id, model);
                        versionTrans.Assign(id, new EntityVersion(id, 1, DateTime.UtcNow, CurrentAgent.Id, false));

                        return;
                    }

                    advanceVersions(id, currentModel.Model, isDeleting: false);

                    // Update the original id with the latest version.
                    modelTrans.Assign(id, model);
                },
                unassignAction: id =>
                {
                    var currentModel = modelTrans.ExistingModels.Execute(id);

                    if (currentModel == null)
                    {
                        // Nothing to do if we're trying to delete a model doesn't exist.
                        return;
                    }

                    advanceVersions(id, currentModel.Model, isDeleting: true);

                    // Unassign the original ID. The version record will act as a gravestone.
                    modelTrans.Unassign(id);
                },
                applyChangesAction: () =>
                {
                    return modelTrans.ApplyChanges().Union(versionTrans.ApplyChanges()).ToList();
                },
                existingModels: ExistingModels
            );
        }
    }
}
