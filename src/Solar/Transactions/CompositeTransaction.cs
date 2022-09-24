﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Transactions
{
    public class CompositeTransaction : ITransaction
    {
        private IEnumerable<ITransaction> Transactions;

        public CompositeTransaction(IEnumerable<ITransaction> transactions)
        {
            this.Transactions = transactions;
        }

        public bool IsValid => Transactions.All(o => o.IsValid);

        public IEnumerable<ICommitable> ApplyChanges()
        {
            return Transactions.SelectMany(o => o.ApplyChanges()).ToList().Distinct();
        }

        public void Invalidate()
        {
            Transactions.ForEach(o => o.Invalidate());
        }
    }
}
