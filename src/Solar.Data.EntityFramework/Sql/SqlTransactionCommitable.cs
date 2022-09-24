using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework.Sql
{
    public class SqlTransactionCommitable : ICommitable
    {
        private SqlTransaction Transaction;
        private bool IsCommited;
        private Action CleanUp;

        public SqlTransactionCommitable(SqlTransaction transaction, Action cleanUp)
        {
            this.Transaction = transaction;
            this.IsCommited = false;
            this.CleanUp = cleanUp;
        }

        public void Commit()
        {
            IsCommited = true;

            try
            {
                Transaction.Commit();
            }
            catch (Exception)
            {
                Transaction.Rollback();
                throw;
            }
            finally
            {
                CleanUp();
            }
        }

        ~SqlTransactionCommitable()
        {
            if (!IsCommited)
            {
                Transaction.Rollback();
                CleanUp();
            }
        }
    }
}
