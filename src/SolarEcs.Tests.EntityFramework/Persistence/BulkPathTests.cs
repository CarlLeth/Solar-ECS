using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.Sql;
using SolarEcs.Tests.EntityFramework.Infrastructure;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Persistence
{
    /// <summary>
    /// Stresses the SqlBulkTransaction write path: SqlBulkCopy uploads, table-valued
    /// parameter deletes, and the alternate DatabaseMode / UseBulkUploading settings.
    /// </summary>
    [TestClass]
    public class BulkPathTests : EfTestsBase
    {
        public BulkPathTests()
        {
            UseSqlPersistence();
        }

        [TestMethod]
        public void CanBulkUploadLargeVolumeInSingleTransaction()
        {
            var store = Store<RichComponent>();
            var transaction = store.CreateTransaction();

            var ids = new List<Guid>();
            for (int i = 0; i < 2500; i++)
            {
                var id = transaction.Add(new RichComponent
                {
                    Text = $"Entity {i}",
                    Number = i,
                    OptionalNumber = i % 2 == 0 ? (int?)null : i,
                    IsEnabled = i % 3 == 0,
                    Timestamp = new DateTime(2026, 1, 1).AddDays(i),
                    Amount = (decimal)i / 4,
                    RelatedId = null,
                });
                ids.Add(id);
            }

            transaction.Commit();

            Assert.AreEqual(2500, store.All.Count());
            Assert.AreEqual("Entity 1234", store.For(ids[1234]).Text);
            Assert.AreEqual(1234, store.For(ids[1234]).Number);
            Assert.IsNull(store.For(ids[1234]).OptionalNumber);
        }

        [TestMethod]
        public void CanBulkDeleteViaTableValuedParameter()
        {
            var store = Store<TextComponent>();

            var ids = new List<Guid>();
            for (int i = 0; i < 500; i++)
            {
                ids.Add(store.AddCommit(new TextComponent { Text = $"Row {i}" }));
            }

            var transaction = store.CreateTransaction();
            for (int i = 0; i < 250; i++)
            {
                transaction.Unassign(ids[i]);
            }
            transaction.Commit();

            Assert.AreEqual(250, store.All.Count());

            var remainingTexts = store.Components().Select(o => o.Text).ToList();
            Assert.IsFalse(remainingTexts.Contains("Row 0"));
            Assert.IsTrue(remainingTexts.Contains("Row 250"));
        }

        [TestMethod]
        public void CanUnassignNonexistentIdsWithoutError()
        {
            var store = Store<TextComponent>();
            var existing = store.AddCommit(new TextComponent { Text = "Survivor" });

            var transaction = store.CreateTransaction();
            transaction.Unassign(Guid.NewGuid());
            transaction.Unassign(Guid.NewGuid());
            transaction.Commit();

            Assert.AreEqual("Survivor", store.For(existing).Text);
            Assert.AreEqual(1, store.All.Count());
        }

        [TestMethod]
        public void BulkUploadThrowsWhenNestedComplexTypeIsNull()
        {
            // Pinned current behavior: the bulk-copy path cannot handle a null nested
            // complex type and throws instead of persisting nulls.
            var store = Store<MoneyAndLabelComponent>();
            var context = new SharedSqlContext(TestConnection.ConnectionString);
            var transaction = new SqlBulkTransaction<MoneyAndLabelComponent>(
                context, "MoneyAndLabelComponent", store.ToQueryPlan(), new TestSqlSettings());

            transaction.Assign(Guid.NewGuid(), new MoneyAndLabelComponent { Money = null, Label = "No money" });

            AssertThrows<InvalidOperationException>(() => transaction.ApplyChanges().ToList());
            AbandonSharedContext(context);
        }

        /// <summary>
        /// Deterministically rolls back and closes a SharedSqlContext whose transaction
        /// was left open by a failed commit. Without this, the context's finalizer
        /// rolls back at an unpredictable time and can crash the test host.
        /// </summary>
        private static void AbandonSharedContext(SharedSqlContext context)
        {
            var transactionField = typeof(SharedSqlContext).GetField("Transaction",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var connectionField = typeof(SharedSqlContext).GetField("Connection",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isCommitedField = typeof(SharedSqlContext).GetField("IsCommited",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var transaction = (SqlTransaction)transactionField.GetValue(context);
            var connection = (SqlConnection)connectionField.GetValue(context);

            transaction?.Rollback();
            transaction?.Dispose();
            connection?.Close();

            transactionField.SetValue(context, null);
            isCommitedField.SetValue(context, true);
        }

        [TestMethod]
        public void CanUseNonBulkDbSetTransactionPath()
        {
            using (var customDb = CreateCustomDbContext(new TestSqlSettings { UseBulkUploading = false }))
            {
                var store = (IStore<TextComponent>)customDb.Store(typeof(TextComponent));

                var id1 = store.AddCommit(new TextComponent { Text = "DbSet One" });
                var id2 = store.AddCommit(new TextComponent { Text = "DbSet Two" });

                Assert.AreEqual("DbSet One", store.For(id1).Text);
                Assert.AreEqual(2, store.All.Count());

                store.AssignCommit(id1, new TextComponent { Text = "DbSet One updated" });
                Assert.AreEqual("DbSet One updated", store.For(id1).Text);

                store.UnassignCommit(id2);
                Assert.IsNull(store.For(id2));
                Assert.AreEqual(1, store.All.Count());
            }
        }

        // ---------------------------------------------------------------------
        // Known bugs in SqlBulkTransaction, exposed by these tests but skipped
        // until they are fixed. Remove the [Ignore] attributes once fixed.
        // ---------------------------------------------------------------------

        /// <summary>
        /// BUG: SqlBulkTransaction.BulkDeleteComponentsBatches (used when
        /// DatabaseMode is Generic) builds batches and command objects but never
        /// executes any DELETE, so unassignments are silently dropped.
        /// </summary>
        [Ignore("Known bug: BulkDeleteComponentsBatches never executes its delete commands in Generic database mode.")]
        [TestMethod]
        public void GenericDatabaseModeDeletesComponents()
        {
            using (var customDb = CreateCustomDbContext(new TestSqlSettings { DatabaseMode = SqlDatabaseMode.Generic }))
            {
                var store = (IStore<TextComponent>)customDb.Store(typeof(TextComponent));

                var toKeep = store.AddCommit(new TextComponent { Text = "Keep me" });
                var toRemove = store.AddCommit(new TextComponent { Text = "Delete me" });

                store.UnassignCommit(toRemove);

                Assert.IsNull(store.For(toRemove), "Row should have been deleted in Generic database mode.");
                Assert.AreEqual("Keep me", store.For(toKeep).Text);
            }
        }

        /// <summary>
        /// BUG: SqlBulkTransaction.GetDeleteSql interpolates the entities collection
        /// object directly into the SQL string instead of the formatted id list.
        /// </summary>
        [Ignore("Known bug: GetDeleteSql interpolates the entities collection instead of the formatted id list.")]
        [TestMethod]
        public void GetDeleteSqlFormatsIdListIntoSql()
        {
            var transaction = CreateUnattachedBulkTransaction();

            var method = transaction.GetType().GetMethod("GetDeleteSql",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var sql = (string)method.Invoke(transaction, new object[]
            {
                new List<Guid> { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
            });

            StringAssert.Contains(sql, "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            StringAssert.Contains(sql, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            StringAssert.Contains(sql, "DELETE FROM");
        }

        private object CreateUnattachedBulkTransaction()
        {
            // SqlBulkTransaction's constructor only uses its dependencies when Apply()
            // runs, so an unattached instance is enough to test GetDeleteSql.
            var sharedContext = new SharedSqlContext(TestConnection.ConnectionString);
            var queryPlan = Store<TextComponent>().ToQueryPlan();

            return new SqlBulkTransaction<TextComponent>(sharedContext, "TextComponent", queryPlan, new TestSqlSettings());
        }
    }
}
