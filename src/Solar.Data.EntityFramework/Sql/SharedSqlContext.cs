using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework.Sql
{
    public class SharedSqlContext : ICommitable
    {
        private SqlConnection Connection;
        private SqlTransaction Transaction;

        private bool IsCommited;

        public SharedSqlContext(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
        }

        private SqlConnection GetOpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }

            return Connection;
        }

        private SqlTransaction GetTransaction()
        {
            if (Transaction == null)
            {
                var conn = GetOpenConnection();
                Transaction = conn.BeginTransaction();
            }

            return Transaction;
        }

        public void DoInTransaction(Action<SqlTransaction> action)
        {
            action(GetTransaction());
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
                Transaction.Dispose();
                Transaction = null;
                Connection.Close();
            }
        }

        ~SharedSqlContext()
        {
            if (!IsCommited && Transaction != null)
            {
                Transaction.Rollback();
            }

            Connection.Dispose();
        }
    }
}
