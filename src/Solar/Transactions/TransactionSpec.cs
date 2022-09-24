using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Transactions
{
    public class TransactionSpec<TModel>
    {
        public Action<Guid, TModel> Assign { get; private set; }
        public Action<Guid> Unassign { get; private set; }

        public TransactionSpec(Action<Guid, TModel> assign, Action<Guid> unassign)
        {
            this.Assign = assign;
            this.Unassign = unassign;
        }
    }
}
