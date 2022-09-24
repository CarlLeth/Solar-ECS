using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Transactions
{
    public static class Transaction
    {
        public static ITransaction Combine(params ITransaction[] transactions)
        {
            return new CompositeTransaction(transactions);
        }

        public static ITransaction Combine(IEnumerable<ITransaction> transactions)
        {
            return new CompositeTransaction(transactions);
        }

        public static ITransaction Empty()
        {
            return new EmptyTransaction<object>();
        }

        public static ITransaction<TModel> Empty<TModel>()
        {
            return new EmptyTransaction<TModel>();
        }
    }
}
