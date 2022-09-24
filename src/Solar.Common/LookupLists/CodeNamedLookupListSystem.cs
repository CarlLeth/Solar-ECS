using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solar.Common.Identification;

namespace Solar.Common.LookupLists
{
    public class CodeNamedLookupListSystem : ICodeNamedLookupListSystem
    {
        private ILookupListSystem LookupLists;
        private IStore<CodeName> CodeNames;

        public CodeNamedLookupListSystem(ILookupListSystem lookupLists, IStore<CodeName> codeNames)
        {
            this.LookupLists = lookupLists;
            this.CodeNames = codeNames;
        }

        public IQueryPlan<CodeNamedLookupListModel> QueryFor(Guid list)
        {
            return LookupLists.QueryFor(list)
                .EntityJoin(CodeNames.ToQueryPlan(), (lkp, codeName) => new CodeNamedLookupListModel(codeName.Name, lkp.Name, lkp.Description, lkp.Ordinal));
        }

        public IRecipe<CodeNamedLookupListModel> RecipeFor(Guid list)
        {
            return QueryFor(list)
                .StartRecipe()
                .IncludeSimple(LookupLists.RecipeFor(list), model => new LookupListModel(model.Name, model.Description, model.Ordinal))
                .IncludeSimple(CodeNames.ToRecipe(), model => new CodeName(model.CodeName));
        }
    }
}
