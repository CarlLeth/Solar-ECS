using SolarEcs.Common.Identification;
using SolarEcs.Common.Lists;
using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.LookupLists
{
    public class LookupListSystem : ILookupListSystem
    {
        private IListSystem ListSystem;
        private INameSystem NameSystem;
        private ITextSystem TextSystem;

        public LookupListSystem(IListSystem listSystem, INameSystem nameSystem, ITextSystem textSystem)
        {
            this.ListSystem = listSystem;
            this.NameSystem = nameSystem;
            this.TextSystem = textSystem;
        }

        public IQueryPlan<LookupListModel> QueryFor(Guid list)
        {
            var members = ListSystem.Query
                .Where(o => o.Model.List == list)
                .ShiftKey(o => o.Model.Entity);

            return members
                .EntityLeftJoin(NameSystem.Query, (listMember, name) => new { listMember.Ordinal, Name = name.Get(o => o.Name, "") })
                .EntityLeftJoin(TextSystem.Query, (namedMember, text) => new LookupListModel(namedMember.Name, text.Get(o => o.Text), namedMember.Ordinal));
        }

        public IRecipe<LookupListModel> RecipeFor(Guid list)
        {
            return QueryFor(list).CreateRecipeFrom(ListSystem.Recipe, NameSystem.Recipe, TextSystem.Recipe,
                (listTrans, nameTrans, textTrans) => new TransactionSpec<LookupListModel>(
                    assign: (id, model) =>
                    {
                        var listMembership = ListSystem.Query
                            .Where(o => o.Model.List == list && o.Model.Entity == id)
                            .ExecuteKeysOnly()
                            .FirstOrDefault();

                        if (listMembership == Guid.Empty)
                        {
                            listTrans.Add(new ListMembershipModel(id, list, model.Ordinal));
                        }
                        else
                        {
                            listTrans.Assign(listMembership, new ListMembershipModel(id, list, model.Ordinal));
                        }

                        nameTrans.Assign(id, new NameModel(model.Name));

                        if (model.Description != null)
                        {
                            textTrans.Assign(id, new TextModel(model.Description));
                        }
                        else
                        {
                            textTrans.Unassign(id);
                        }
                    },
                    unassign: id =>
                    {
                        listTrans.UnassignWhere(o => o.Entity == id && o.List == list);
                        nameTrans.Unassign(id);
                        textTrans.Unassign(id);
                    }
                )
            );
        }
    }
}
