using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Scripting
{
    public class BasicScriptableRecipe<TModel> : IScriptableRecipe<TModel>
    {
        private IRecipe<TModel> Recipe;

        public BasicScriptableRecipe(IRecipe<TModel> recipe)
        {
            this.Recipe = recipe;
        }

        public ITransaction Apply(ChangeScript<TModel> script)
        {
            var trans = Recipe.CreateTransaction();

            script.Assign?.ForEach(o => trans.Assign(o.Key, o.Value));
            script.Unassign?.ForEach(key => trans.Unassign(key));

            return trans;
        }
    }
}
