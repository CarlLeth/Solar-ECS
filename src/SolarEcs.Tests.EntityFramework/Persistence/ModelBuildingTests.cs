using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.ComponentTableMappers;
using SolarEcs.Tests.EntityFramework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Persistence
{
    /// <summary>
    /// Verifies that registering component packages through the EF provider builds a
    /// correct database schema: one table per component, correctly-named columns
    /// (including complex-type flattening), and proper persistence type generation.
    /// </summary>
    [TestClass]
    public class ModelBuildingTests : EfTestsBase
    {
        public ModelBuildingTests()
        {
            UseSqlPersistence();
        }

        [TestMethod]
        public void AllRegisteredComponentTypesHavePersistenceTypes()
        {
            Assert.IsTrue(Catalog.Contains<TextComponent>());
            Assert.IsTrue(Catalog.Contains<RichComponent>());
            Assert.IsTrue(Catalog.Contains<MoneyAndLabelComponent>());
            Assert.IsTrue(Catalog.Contains<BlobComponent>());

            var available = Catalog.AvailableComponentTypes.ToList();
            CollectionAssert.IsSubsetOf(
                new List<Type>
                {
                    typeof(TextComponent),
                    typeof(RichComponent),
                    typeof(MoneyAndLabelComponent),
                    typeof(BlobComponent),
                },
                available);
        }

        [TestMethod]
        public void ComponentTablesAreCreatedInDatabase()
        {
            var tables = ListTables();

            Assert.IsTrue(tables.Contains("TextComponent"), $"TextComponent table missing. Tables: {string.Join(", ", tables)}");
            Assert.IsTrue(tables.Contains("RichComponent"));
            Assert.IsTrue(tables.Contains("MoneyAndLabelComponent"));
            Assert.IsTrue(tables.Contains("BlobComponent"));
        }

        [TestMethod]
        public void SimpleAndNullableColumnsAreMapped()
        {
            var columns = ListColumns("RichComponent");

            foreach (var expected in new[] { "Id", "Text", "Number", "OptionalNumber", "IsEnabled", "Timestamp", "Amount", "RelatedId" })
            {
                Assert.IsTrue(columns.Contains(expected), $"Column '{expected}' missing on RichComponent. Columns: {string.Join(", ", columns)}");
            }
        }

        [TestMethod]
        public void ComplexTypePropertiesAreFlattenedIntoColumns()
        {
            var columns = ListColumns("MoneyAndLabelComponent");

            Assert.IsTrue(columns.Contains("Label"));
            Assert.IsTrue(columns.Contains("Money_Currency"), $"Complex type 'Money' should be flattened to Money_* columns. Columns: {string.Join(", ", columns)}");
            Assert.IsTrue(columns.Contains("Money_Amount"));
        }

        [TestMethod]
        public void ComponentTableHasGuidIdPrimaryKey()
        {
            var schema = TestDb.Database.SqlQuery<string>(
                "SELECT c.name FROM sys.indexes i JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id " +
                "JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id " +
                "JOIN sys.types t ON c.system_type_id = t.system_type_id " +
                "WHERE i.is_primary_key = 1 AND i.object_id = OBJECT_ID('dbo.TextComponent')").ToList();

            Assert.AreEqual(1, schema.Count);
            Assert.AreEqual("Id", schema[0]);
        }

        [TestMethod]
        public void CanQueryThroughStoreAfterModelBuilding()
        {
            var count = Store<TextComponent>().All.Count();
            Assert.AreEqual(0, count);
        }

        private List<string> ListTables()
        {
            return TestDb.Database.SqlQuery<string>(
                "SELECT t.name FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id WHERE s.name = 'dbo'").ToList();
        }

        private List<string> ListColumns(string tableName)
        {
            return TestDb.Database.SqlQuery<string>(
                "SELECT c.name FROM sys.columns c WHERE c.object_id = OBJECT_ID(@p0)", tableName).ToList();
        }
    }

    /// <summary>
    /// Verifies the SchemaComponentTableMapper: component tables land in a schema
    /// derived from the first component namespace. Uses a dedicated context subclass
    /// because EF6 caches the compiled model per context type, and this class needs a
    /// model with a different table-mapping configuration.
    /// </summary>
    [TestClass]
    public class SchemaTableMappingTests
    {
        private class SchemaMappedSandboxDbContext : SandboxComponentDbContext
        {
            public SchemaMappedSandboxDbContext(string nameOrConnectionString, IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper)
                : base(nameOrConnectionString, persistenceTypeLibrary, componentTableMapper)
            { }
        }

        [TestMethod]
        public void SchemaMapperPlacesTablesInNamespaceDerivedSchema()
        {
            System.Data.Entity.Database.SetInitializer(
                new DropCreateDatabaseAlways<SchemaMappedSandboxDbContext>());

            var mapper = new SchemaComponentTableMapper();
            var connectionString = new System.Data.SqlClient.SqlConnectionStringBuilder(TestConnection.ConnectionString)
            {
                InitialCatalog = "SolarSchemaTests"
            }.ConnectionString;

            using (var db = new SchemaMappedSandboxDbContext(connectionString, new PersistenceTypeLibrary(), mapper))
            {
                db.RegisterComponentPackage(new TestComponentPackage(), "Test");

                // Touch the database to force model creation and database initialization.
                var tableCount = db.Database.SqlQuery<int>(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'test'").First();

                Assert.IsTrue(tableCount > 0, "No tables were created in the 'test' schema.");

                var tables = db.Database.SqlQuery<string>(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'test'").ToList();

                CollectionAssert.Contains(tables, "TextComponent");
                CollectionAssert.Contains(tables, "RichComponent");
            }
        }
    }
}
