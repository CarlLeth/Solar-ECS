using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Data.EntityFramework.Sql;
using SolarEcs.Tests.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Persistence
{
    /// <summary>
    /// Exercises the SharedSqlContext transaction machinery and cross-store
    /// transaction behavior, pinning current semantics before an EF Core migration.
    /// </summary>
    [TestClass]
    public class TransactionTests : EfTestsBase
    {
        public TransactionTests()
        {
            UseSqlPersistence();
        }

        [TestMethod]
        public void SharedSqlContextCommitsRawWrites()
        {
            var context = new SharedSqlContext(TestConnection.ConnectionString);

            context.DoInTransaction(transaction =>
            {
                var cmd = transaction.Connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = "INSERT INTO TextComponent (Id, Text) VALUES (@id, @text)";
                cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@text", "Raw insert");
                cmd.ExecuteNonQuery();
            });
            context.Commit();

            Assert.AreEqual(1, Store<TextComponent>().All.Count());
        }

        [TestMethod]
        public void SharedSqlContextCanBeReusedAfterCommit()
        {
            var context = new SharedSqlContext(TestConnection.ConnectionString);

            context.DoInTransaction(transaction => InsertRawRow(transaction, "First"));
            context.Commit();

            context.DoInTransaction(transaction => InsertRawRow(transaction, "Second"));
            context.Commit();

            Assert.AreEqual(2, Store<TextComponent>().All.Count());
        }

        [TestMethod]
        public void TransactionScopeRollbackDiscardsBulkWrites()
        {
            // ITransactionExtensions.Commit wraps work in a TransactionScope of its own;
            // an outer scope that is never completed must roll everything back.
            var store = Store<TextComponent>();

            using (var outerScope = new System.Transactions.TransactionScope())
            {
                store.AddCommit(new TextComponent { Text = "Inside outer scope" });

                // Deliberately do not call outerScope.Complete().
            }

            Assert.AreEqual(0, store.All.Count());
        }

        [TestMethod]
        public void SequentialTransactionsAcrossStoresPersistSeparately()
        {
            var textStore = Store<TextComponent>();
            var richStore = Store<RichComponent>();

            textStore.AddCommit(new TextComponent { Text = "Text store write" });
            richStore.AddCommit(new RichComponent { Text = "Rich store write", Number = 1 });

            Assert.AreEqual(1, textStore.All.Count());
            Assert.AreEqual(1, richStore.All.Count());
        }

        [TestMethod]
        public void InvalidatedTransactionDoesNotPersistChanges()
        {
            var store = Store<TextComponent>();
            var transaction = store.CreateTransaction();
            transaction.Add(new TextComponent { Text = "Should never persist" });
            transaction.Invalidate("Deliberately invalidated");

            AssertThrows<InvalidOperationException>(() => transaction.Commit());

            Assert.AreEqual(0, store.All.Count());
        }

        private void InsertRawRow(SqlTransaction transaction, string text)
        {
            var cmd = transaction.Connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "INSERT INTO TextComponent (Id, Text) VALUES (@id, @text)";
            cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@text", text);
            cmd.ExecuteNonQuery();
        }
    }
}
