using SolarEcs.Data.EntityFramework.ComponentTableMappers;
using SolarEcs.Data.EntityFramework.Sql;
using SolarEcs.Construction;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework
{
    public class ComponentDbContext : DbContext, IComponentCatalog, ICommitable
    {
        private static readonly MethodInfo StoreMethod = typeof(ComponentDbContext).GetMethod(nameof(CreateDbSetStoreGeneric), BindingFlags.NonPublic | BindingFlags.Instance);

        protected ISet<Type> RegisteredComponentTypes { get; private set; }
        protected IPersistenceTypeLibrary PersistenceTypeLibrary { get; private set; }

        private List<Tuple<IComponentPackage, IEnumerable<string>>> RegisteredPackages;
        private IComponentTableMapper ComponentTableMapper;
        private ISolarSqlSettings SqlSettings;

        private SharedSqlContext SqlContext;

        public ComponentDbContext(DbConnection connection, IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper, ISolarSqlSettings sqlSettings = null)
            : base(connection, false)
        {
            Initialize(persistenceTypeLibrary, componentTableMapper, sqlSettings);
        }

        public ComponentDbContext(string nameOrConnectionString, IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper, ISolarSqlSettings sqlSettings = null)
            : base(nameOrConnectionString)
        {
            Initialize(persistenceTypeLibrary, componentTableMapper, sqlSettings);
        }


        public ComponentDbContext(string nameOrConnectionString, IPersistenceTypeLibrary persistenceTypeLibrary, ISolarSqlSettings sqlSettings = null)
            : base(nameOrConnectionString)
        {
            Initialize(persistenceTypeLibrary, null, sqlSettings);
        }

        public ComponentDbContext(IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper, ISolarSqlSettings sqlSettings = null)
            : base()
        {
            Initialize(persistenceTypeLibrary, componentTableMapper, sqlSettings);
        }

        public ComponentDbContext(IPersistenceTypeLibrary persistenceTypeLibrary, ISolarSqlSettings sqlSettings = null)
            : base()
        {
            Initialize(persistenceTypeLibrary, null, sqlSettings);
        }

        private void Initialize(IPersistenceTypeLibrary persistenceTypeLibrary, IComponentTableMapper componentTableMapper, ISolarSqlSettings sqlSettings)
        {
            this.PersistenceTypeLibrary = persistenceTypeLibrary;
            this.ComponentTableMapper = componentTableMapper ?? new PrefixComponentTableMapper();

            this.RegisteredComponentTypes = new HashSet<Type>();
            this.RegisteredPackages = new List<Tuple<IComponentPackage, IEnumerable<string>>>();

            this.SqlSettings = sqlSettings ?? new StandardSolarSqlSettings();

            base.Configuration.AutoDetectChangesEnabled = false;
            base.Configuration.ProxyCreationEnabled = false;
            base.Database.CommandTimeout = SqlSettings.CommandTimeout;

            this.SqlContext = new SharedSqlContext(Database.Connection.ConnectionString);
        }

        public void RegisterComponentPackage(IComponentPackage package, params string[] componentNamespace)
        {
            var registration = new ComponentRegistration(RegisteredComponentTypes);
            package.Register(registration);

            foreach (var componentType in RegisteredComponentTypes)
            {
                PersistenceTypeLibrary.CreatePersistenceType(componentType);
            }

            RegisteredPackages.Add(new Tuple<IComponentPackage, IEnumerable<string>>(package, componentNamespace));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var packageNamespace in RegisteredPackages)
            {
                var registration = new ComponentModelBuilderRegistration(PersistenceTypeLibrary, modelBuilder, ComponentTableMapper, packageNamespace.Item2);
                registration.Package(packageNamespace.Item1);
            }

            RegisteredPackages.Clear();
        }

        public IStore Store(Type componentType)
        {
            return (IStore)CreateDbSetStore(PersistenceTypeLibrary.GetPersistedType(componentType), componentType);
        }

        private object CreateDbSetStore(Type persistedType, Type componentType)
        {
            var method = StoreMethod.MakeGenericMethod(persistedType, componentType);
            return method.Invoke(this, new object[] { });
        }

        private DbSetStore<TPersisted, TComponent> CreateDbSetStoreGeneric<TPersisted, TComponent>()
            where TPersisted : EntityWith<TComponent>
            where TComponent : class
        {
            Expression<Func<TPersisted, TComponent>> persistedToComponent = o => o.Component;
            var componentToPersisted = CreateComponentToPersistedExpression(typeof(TPersisted), typeof(TComponent)) as Expression<Func<TComponent, TPersisted>>;

            return new DbSetStore<TPersisted, TComponent>(
                Set<TPersisted>(),
                this,
                GetTableName<TPersisted>(),
                SqlContext,
                persistedToComponent,
                componentToPersisted,
                SqlSettings
            );
        }

        private LambdaExpression CreateComponentToPersistedExpression(Type persistedType, Type componentType)
        {
            var param = Expression.Parameter(componentType);

            var newExpr = Expression.New(persistedType);

            var member = persistedType.GetProperty("Component");
            var binding = Expression.Bind(member, param);

            var body = Expression.MemberInit(newExpr, binding);
            return Expression.Lambda(body, param);
        }

        public bool Contains(Type componentType)
        {
            return RegisteredComponentTypes.Contains(componentType);
        }

        public IEnumerable<Type> AvailableComponentTypes
        {
            get { return RegisteredComponentTypes; }
        }

        void ICommitable.Commit()
        {
            SaveChanges();
        }

        // See https://stackoverflow.com/questions/1895455/get-database-table-name-from-entity-framework-metadata
        private string GetTableName<T>() where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)this).ObjectContext;

            return GetTableName<T>(objectContext);
        }

        private static string GetTableName<T>(ObjectContext context) where T : class
        {
            string sql = context.CreateObjectSet<T>().ToTraceString();
            Regex regex = new Regex(@"FROM\s+(?<table>.+)\s+AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        /// <summary>
        /// Responsible for denoting which component types have been registered.  Always runs.
        /// </summary>
        private class ComponentRegistration : IComponentRegistration
        {
            private ISet<Type> RegisteredComponentTypes { get; set; }

            public ComponentRegistration(ISet<Type> registeredComponentTypes)
            {
                this.RegisteredComponentTypes = registeredComponentTypes;
            }

            public void Component<TComponent>() where TComponent : class
            {
                RegisteredComponentTypes.Add(typeof(TComponent));                
            }

            public void ValueType<TValueType>() where TValueType : class
            {
                //Do nothing
            }

            public void Package(IComponentPackage package, params string[] childNamespace)
            {
                //We don't care about namespaces, only which component types are registered.
                package.Register(this);
            }
        }

        /// <summary>
        /// Responsible for registering components with the DbModelBuilder.  Only runs on model creation.
        /// </summary>
        private class ComponentModelBuilderRegistration : IComponentRegistration
        {
            private static readonly MethodInfo AssignMethod = typeof(ComponentModelBuilderRegistration).GetMethod(nameof(AssignTypeGeneric), BindingFlags.NonPublic | BindingFlags.Instance);

            private IPersistenceTypeLibrary PersistenceTypeLibrary;
            private DbModelBuilder ModelBuilder;
            private IComponentTableMapper ComponentTableMapper;
            private IEnumerable<string> ComponentNamespace;

            public ComponentModelBuilderRegistration(IPersistenceTypeLibrary persistenceTypeLibrary, DbModelBuilder modelBuilder, IComponentTableMapper componentTableMapper,
                IEnumerable<string> componentNamespace)
            {
                this.PersistenceTypeLibrary = persistenceTypeLibrary;
                this.ModelBuilder = modelBuilder;
                this.ComponentTableMapper = componentTableMapper;
                this.ComponentNamespace = componentNamespace;
            }

            public void Component<TComponent>()
                where TComponent : class
            {
                ModelBuilder.ComplexType<TComponent>();
                AssignType(PersistenceTypeLibrary.GetPersistedType<TComponent>(), typeof(TComponent));
            }

            public void ValueType<TValueType>()
                where TValueType : class
            {
                ModelBuilder.ComplexType<TValueType>();
            }

            private void AssignType(Type persistedType, Type componentType)
            {
                var method = AssignMethod.MakeGenericMethod(persistedType);
                method.Invoke(this, new object[] { componentType });
            }

            private void AssignTypeGeneric<TPersisted>(Type componentType)
                where TPersisted : class
            {
                ComponentTableMapper.MapComponentToTable<TPersisted>(componentType.Name, ComponentNamespace, ModelBuilder);
                var mapper = new PersistedTypeColumnNameMapper<TPersisted>(ModelBuilder, componentType);
                mapper.Map();
            }

            public void Package(IComponentPackage package, params string[] childNamespace)
            {
                var fullChildNamespace = ComponentNamespace.Concat(childNamespace);

                var childRegistration = new ComponentModelBuilderRegistration(PersistenceTypeLibrary, ModelBuilder, ComponentTableMapper, fullChildNamespace);
                package.Register(childRegistration);
            }
        }
    }
}
