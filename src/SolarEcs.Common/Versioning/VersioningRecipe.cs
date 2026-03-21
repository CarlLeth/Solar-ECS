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

        public VersioningRecipe(IRecipe<T> modelRecipe, IStore<EntityVersion> entityVersions, VersioningSystem versioningSystem, IDataAgent currentAgent)
        {
            ModelRecipe = modelRecipe;
            EntityVersions = entityVersions;
            VersioningSystem = versioningSystem;
            CurrentAgent = currentAgent;
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
                    // Existing model, but no version information has been saved. Create the current version with unknown author and time.
                    currentVersion = new EntityVersion(id, 1, null, null, false);
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
