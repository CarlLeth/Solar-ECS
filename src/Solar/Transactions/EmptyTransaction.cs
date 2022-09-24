using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Transactions
{
    public class EmptyTransaction : ITransaction
    {
        public EmptyTransaction()
        {
            IsValid = true;
        }

        public IEnumerable<ICommitable> ApplyChanges()
        {
            return Enumerable.Empty<ICommitable>();
        }

        public bool IsValid { get; private set; }

        public void Invalidate()
        {
            IsValid = false;
        }
    }
}
