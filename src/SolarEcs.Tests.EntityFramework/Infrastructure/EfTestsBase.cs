using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.ComponentTableMappers;
using SolarEcs.Data.EntityFramework.Sql;
using SolarEcs.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Infrastructure
{
    /// <summary>
    /// Base class for tests that exercise Solar's EF6 SQL Server provider for real.
    /// Each test class gets its own freshly-created database (via DropCreateDatabaseAlways).
    /// </summary>
    public abstract class EfTestsBase : DomainTestsBase
    {
        public EfTestsBase()
        {
            AddComponentPackage(new TestComponentPackage());
        }

        /// <summary>
        /// Clears all component data before every test. EF6 only runs its database
        /// initializer once per AppDomain per context type, so DropCreateDatabaseAlways
        /// does not give each test a fresh database; truncating through the sandbox
        /// context keeps every test isolated on the same schema.
        /// </summary>
        [TestInitialize]
        public void ClearComponentDataBetweenTests()
        {
            TestDb?.PermanentlyDestroyAllData();
        }

        protected void UseSqlPersistence()
        {
            UseEntityFrameworkPersistence(SqlDatabaseMode.SqlServer);
        }

        protected override SandboxComponentDbContext CreateDbContext(IPersistenceTypeLibrary persistenceTypeLibrary)
        {
            return new SandboxComponentDbContext(
                TestConnection.ConnectionString, persistenceTypeLibrary, new PrefixComponentTableMapper());
        }

        /// <summary>
        /// Creates an additional context against the same database with custom SQL settings,
        /// used to exercise alternate code paths (e.g. the non-bulk DbSetTransaction path,
        /// or Generic database mode). Registers the same component package so it shares
        /// the model used by the rest of the test run.
        /// </summary>
        protected ComponentDbContext CreateCustomDbContext(ISolarSqlSettings settings, string databaseName = null)
        {
            var connectionString = string.IsNullOrEmpty(databaseName)
                ? TestConnection.ConnectionString
                : ReplaceDatabaseName(TestConnection.ConnectionString, databaseName);

            var db = new ComponentDbContext(
                connectionString, SharedPersistenceTypeLibrary, new PrefixComponentTableMapper(), settings);

            db.RegisterComponentPackage(new TestComponentPackage());

            return db;
        }

        private static string ReplaceDatabaseName(string connectionString, string databaseName)
        {
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = databaseName;
            return builder.ConnectionString;
        }
    }

    /// <summary>
    /// SQL settings used to drive alternate EF provider code paths from tests.
    /// </summary>
    public class TestSqlSettings : ISolarSqlSettings
    {
        public int CommandTimeout { get; set; } = 600;
        public bool UseBulkUploading { get; set; } = true;
        public SqlDatabaseMode DatabaseMode { get; set; } = SqlDatabaseMode.SqlServer;
    }
}
