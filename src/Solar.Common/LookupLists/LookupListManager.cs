using SolarEcs.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.LookupLists
{
    public class LookupListManager
    {
        private ILookupListSystem LookupLists;
        private ICodeNamedLookupListSystem CodeNamedLists;
        private ILookupListEntities LookupListEntities;

        public LookupListManager(ILookupListSystem lookupLists, ICodeNamedLookupListSystem codeNamedLists, ILookupListEntities lookupListEntities)
        {
            this.LookupLists = lookupLists;
            this.CodeNamedLists = codeNamedLists;
            this.LookupListEntities = lookupListEntities;
        }

        #region Managing all lists

        private IQueryPlan<CodeNamedLookupListModel> ListQuery()
        {
            return CodeNamedLists.QueryFor(LookupListEntities.AllLookups);
        }

        private IRecipe<CodeNamedLookupListModel> ListRecipe()
        {
            return CodeNamedLists.RecipeFor(LookupListEntities.AllLookups);
        }

        public IDictionary<Guid, CodeNamedLookupListModel> GetLists()
        {
            return ListQuery().ExecuteToDictionary();
        }

        public void Script(ChangeScript<CodeNamedLookupListModel> script)
        {
            ListRecipe().AsScriptable().ApplyCommit(script);
        }

        public void Assign(Guid id, CodeNamedLookupListModel model)
        {
            ListRecipe().AssignCommit(id, model);
        }

        public Guid Add(CodeNamedLookupListModel model)
        {
            return ListRecipe().AddCommit(model);
        }

        public void Unassign(Guid id)
        {
            var itemTrans = LookupLists.RecipeFor(id).CreateTransaction();
            itemTrans.UnassignWhere(o => true);
            itemTrans.Commit();

            ListRecipe().UnassignCommit(id);
        }

        #endregion

        #region Individual lists

        private IQueryPlan<LookupListModel> QueryFor(string lookupListName)
        {
            var list = LookupListByName(lookupListName);
            return LookupLists.QueryFor(list);
        }

        private IRecipe<LookupListModel> RecipeFor(string lookupListName)
        {
            var list = LookupListByName(lookupListName);
            return LookupLists.RecipeFor(list);
        }

        private Guid LookupListByName(string lookupListName)
        {
            Guid nameAsGuid;

            if (Guid.TryParse(lookupListName, out nameAsGuid))
            {
                return nameAsGuid;
            }

            var list = CodeNamedLists.QueryFor(LookupListEntities.AllLookups)
                .Where(o => o.Model.CodeName == lookupListName)
                .ExecuteAll()
                .FirstOrDefault();

            if (list == null)
            {
                throw new KeyNotFoundException($"No lookup list found matching '{lookupListName}'");
            }

            return list.Key;
        }

        public IDictionary<Guid, LookupListModel> GetItems(string lookupListName)
        {
            return QueryFor(lookupListName).ExecuteToDictionary();
        }

        public LookupListModel GetSingle(string lookupListName, Guid id)
        {
            return QueryFor(lookupListName).Execute(id)?.Model;
        }

        public void Script(string lookupListName, ChangeScript<LookupListModel> script)
        {
            RecipeFor(lookupListName).AsScriptable().ApplyCommit(script);
        }

        public void Assign(string lookupListName, Guid id, LookupListModel model)
        {
            RecipeFor(lookupListName).AssignCommit(id, model);
        }

        public Guid Add(string lookupListName, LookupListModel model)
        {
            return RecipeFor(lookupListName).AddCommit(model);
        }

        public void Unassign(string lookupListName, Guid id)
        {
            RecipeFor(lookupListName).UnassignCommit(id);
        }

        #endregion
    }
}
