using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Scripting
{
    public class IncludeScriptableRecipe<TModel, TInclude> : IScriptableRecipe<TModel>
    {
        private IScriptableRecipe<TModel> BaseRecipe;
        private IRecipe<TInclude> IncludedRecipe;
        private Action<ITransaction<TInclude>, IEnumerable<IKeyWith<Guid, TModel>>, IEnumerable<Guid>> Action;

        public IncludeScriptableRecipe(IScriptableRecipe<TModel> baseRecipe, IRecipe<TInclude> includedRecipe,
            Action<ITransaction<TInclude>, IEnumerable<IKeyWith<Guid, TModel>>, IEnumerable<Guid>> action)
        {
            this.BaseRecipe = baseRecipe;
            this.IncludedRecipe = includedRecipe;
            this.Action = action;
        }

        public ITransaction Apply(ChangeScript<TModel> script)
        {
            var trans = IncludedRecipe.CreateTransaction();

            Action(trans, script.Assign.Select(o => new KeyWith<Guid, TModel>(o.Key, o.Value)), script.Unassign);
            var baseTrans = BaseRecipe.Apply(script);

            return Transaction.Combine(trans, baseTrans);
        }
    }
}
