using SolarEcs.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework.Sql
{
    public class SqlBulkTransaction<TComponent> : CachingComponentTransactionBase<TComponent>
    {
        private SharedSqlContext Context;
        private string TableName;
        private ISolarSqlSettings Settings;

        public SqlBulkTransaction(SharedSqlContext context, string tableName, IQueryPlan<TComponent> existingModels, ISolarSqlSettings settings)
            : base(existingModels)
        {
            this.Context = context;
            this.TableName = tableName;
            this.Settings = settings;
        }

        protected override IEnumerable<ICommitable> Apply(IEnumerable<IKeyWith<Guid, TComponent>> assignments, IEnumerable<Guid> unassignments)
        {
            Context.DoInTransaction(transaction =>
            {
                var toDelete = assignments.Select(o => o.Key).Concat(unassignments);

                BulkDeleteComponents(toDelete, transaction);

                if (assignments.Any())
                {
                    BulkUploadComponents(assignments, transaction);
                }
            });

            yield return Context;
        }

        private string GetDeleteSql(IEnumerable<Guid> entities)
        {
            if (!entities.Any())
            {
                return null;
            }

            var idList = string.Join(",", entities.Select(o => $"'{o}'"));
            return $"DELETE FROM {TableName} WHERE ID IN ({entities})";
        }

        private void BulkDeleteComponents(IEnumerable<Guid> entities, SqlTransaction transaction)
        {
            if (Settings.DatabaseMode == SqlDatabaseMode.SqlServer)
            {
                BulkDeleteComponentsSqlServer(entities, transaction);
            }
            else
            {
                BulkDeleteComponentsBatches(entities, transaction);
            }
        }

        private void BulkDeleteComponentsSqlServer(IEnumerable<Guid> entities, SqlTransaction transaction)
        {
            var cmd = transaction.Connection.CreateCommand();
            cmd.CommandTimeout = Settings.CommandTimeout;
            cmd.Transaction = transaction;

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(Guid));

            foreach (var id in entities)
            {
                var row = dt.Rows.Add(id);
            }

            var tableParam = cmd.Parameters.AddWithValue("@ids", dt);
            tableParam.SqlDbType = SqlDbType.Structured;
            tableParam.TypeName = "EntityListType";

            cmd.CommandText = $"DELETE FROM {TableName} WHERE ID IN (select ID from @ids)";

            cmd.ExecuteNonQuery();
        }

        private void BulkDeleteComponentsBatches(IEnumerable<Guid> entities, SqlTransaction transaction)
        {
            var batchedEntities = entities
                .Select((key, num) => new { key, num })
                .GroupBy(o => o.num / 1000);

            foreach (var batch in batchedEntities)
            {
                var cmd = transaction.Connection.CreateCommand();
                cmd.CommandTimeout = Settings.CommandTimeout;
                cmd.Transaction = transaction;
            }
        }

        private void BulkUploadComponents(IEnumerable<IKeyWith<Guid, TComponent>> components, SqlTransaction transaction)
        {
            var properties = typeof(TComponent)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(o => o.CanWrite);

            var propertyColumns = properties.SelectMany(prop => GetDbProperties(prop, "", o => o)).ToList();

            var rows = components.Select(cmp => Enumerable.Repeat<object>(cmp.Key, 1).Concat(
                propertyColumns.Select(prop => prop.Getter(cmp.Model))).ToArray()
            );

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(Guid));

            foreach (var col in propertyColumns)
            {
                dt.Columns.Add(col.Name, Nullable.GetUnderlyingType(col.PropertyType) ?? col.PropertyType);
            }

            foreach (var row in rows)
            {
                var dataRow = dt.Rows.Add(row);
            }

            BulkUpload(dt, transaction);
        }

        private IEnumerable<SqlColumn<TComponent>> GetDbProperties(PropertyInfo property, string parentName, Func<TComponent, object> parentGetter)
        {
            var propertyName = string.IsNullOrEmpty(parentName) ? property.Name : $"{parentName}_{property.Name}";
            Func<TComponent, object> propertyGetter = o =>
            {
                var parent = parentGetter(o);

                if (parent == null)
                {
                    throw new InvalidOperationException($"Complex type '{parentName}' on type '{typeof(TComponent)}' must not be null.");
                }

                return property.GetValue(parent);
            };

            if (IsColumnType(property.PropertyType))
            {
                return Enumerable.Repeat(new SqlColumn<TComponent>(propertyName, property.PropertyType, propertyGetter), 1);
            }
            else
            {
                return property.PropertyType
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(child => child.CanWrite)
                    .SelectMany(child => GetDbProperties(child, propertyName, propertyGetter));
            }
        }

        private bool IsColumnType(Type type)
        {
            return type == typeof(string) ||
                type.IsValueType ||
                (type.IsArray && type.GetArrayRank() == 1 && IsColumnType(type.GetElementType())) ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private void BulkUpload(DataTable data, SqlTransaction transaction)
        {
            var options = SqlBulkCopyOptions.CheckConstraints;

            using (var copy = new SqlBulkCopy(transaction.Connection, options, transaction))
            {
                copy.BulkCopyTimeout = Settings.CommandTimeout;
                copy.DestinationTableName = TableName;

                foreach (var col in data.Columns.Cast<DataColumn>())
                {
                    copy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                };

                copy.WriteToServer(data);
            };
        }
    }
}
