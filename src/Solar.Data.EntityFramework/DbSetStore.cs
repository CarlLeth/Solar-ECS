using Solar.Data.EntityFramework;
using Solar.Data.EntityFramework.Sql;
using Solar.Ecs.Transactions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Data.EntityFramework
{
    public class DbSetStore<TPersisted, TComponent> : IStore<TComponent>
        where TPersisted : EntityWith<TComponent>
        where TComponent : class
    {
        private IDbSet<TPersisted> DbSet;
        private ICommitable Commitable;
        private SharedSqlContext SqlContext;
        private string TableName;
        private ISolarSqlSettings Settings;

        private Lazy<Func<TPersisted, TComponent>> PersistedToComponent;
        private Lazy<Func<TComponent, TPersisted>> ComponentToPersisted;

        public DbSetStore(IDbSet<TPersisted> dbSet, ICommitable commitable, string tableName, SharedSqlContext sqlContext,
            Expression<Func<TPersisted, TComponent>> persistedToComponent, Expression<Func<TComponent, TPersisted>> componentToPersisted, ISolarSqlSettings settings)
        {
            this.DbSet = dbSet;
            this.Commitable = commitable;
            this.TableName = tableName;
            this.SqlContext = sqlContext;
            this.PersistedToComponent = new Lazy<Func<TPersisted, TComponent>>(() => persistedToComponent.Compile());
            this.ComponentToPersisted = new Lazy<Func<TComponent, TPersisted>>(() => componentToPersisted.Compile());
            this.Settings = settings;
        }

        public bool Contains(Guid id)
        {
            return DbSet.Find(id) != null;
        }

        public IQueryable<IEntityWith<TComponent>> All
        {
            get
            {
                return DbSet.AsNoTracking();
            }
        }

        public ITransaction<TComponent> CreateTransaction()
        {
            if (Settings.UseBulkUploading)
            {
                return new SqlBulkTransaction<TComponent>(SqlContext, TableName, this.ToQueryPlan(), Settings);
            }
            else
            {
                return new DbSetTransaction<TPersisted, TComponent>(DbSet, this.ToQueryPlan(), ComponentToPersisted, Commitable);
            }
        }
    }
}
