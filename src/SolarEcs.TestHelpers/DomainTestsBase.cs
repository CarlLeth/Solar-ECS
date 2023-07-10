using Fusic;
using Fusic.BuildStrategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarEcs.Construction;
using SolarEcs.Data.EntityFramework;
using SolarEcs.Data.EntityFramework.Sql;
using SolarEcs.Data.Memory;
using SolarEcs.Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.TestHelpers
{
    public abstract class DomainTestsBase : IRegisterImplementations
    {
        private static IPersistenceTypeLibrary PersistenceTypeLibrary = new PersistenceTypeLibrary();

        protected IPurgeableComponentCatalog Catalog { get; private set; }
        protected ITypeService TypeService { get; private set; }
        protected IFusicContainer Container { get; private set; }
        protected SolarBuildStrategies BuildStrategies { get; private set; }

        protected int ExecutedQueryCount { get; private set; }

        //protected MockClock Clock { get; }

        private SandboxComponentDbContext Db;

        private Action<IComponentPackage> RegisterComponentPackage;
        private List<IComponentPackage> ComponentPackages;
        //private List<IScenario> Scenarios;

        protected DomainTestsBase()
        {
            this.TypeService = new TypeService();
            this.ComponentPackages = new List<IComponentPackage>();
            //this.Scenarios = new List<IScenario>();
            //this.Clock = new MockClock();

            InitializeContainer();
            UseMemoryPersistence();
        }

        protected void UseMemoryPersistence()
        {
            var catalog = new MemoryComponentCatalog();

            this.Catalog = catalog;
            this.RegisterComponentPackage = catalog.RegisterComponentPackage;

            RegisterPackages();
            //Reseed();
        }

        protected void UseEntityFrameworkPersistence(SqlDatabaseMode mode = SqlDatabaseMode.SqlServer)
        {
            System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseAlways<SandboxComponentDbContext>());
            Db = CreateDbContext(PersistenceTypeLibrary);

            if (mode == SqlDatabaseMode.SqlServer)
            {
                Db.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.types st JOIN sys.schemas ss ON st.schema_id = ss.schema_id WHERE st.name = N'EntityListType' AND ss.name = N'dbo')
                    CREATE TYPE [dbo].[EntityListType] AS TABLE([Id] [uniqueidentifier] NULL)"
                );
            }

            this.Catalog = Db;
            this.RegisterComponentPackage = pkg => Db.RegisterComponentPackage(pkg);

            RegisterPackages();
            //Reseed();
        }

        private void RegisterPackages()
        {
            ComponentPackages.ForEach(package => RegisterComponentPackage(package));
        }

        protected abstract SandboxComponentDbContext CreateDbContext(IPersistenceTypeLibrary persistenceTypeLibrary);

        private void InitializeContainer()
        {
            this.BuildStrategies = new SolarBuildStrategies(() => Catalog, TypeService);

            // Interpret requests like IEnumerable<IAbstraction> as asking for every implementation of the abstraction.
            BuildStrategies.AddInstance(new AllImplementationsBuildStrategy(o => o.IsAbstract));

            RegisterDependencies(BuildStrategies);

            this.Container = BuildStrategies.ToContainer();
        }

        protected virtual void RegisterDependencies(IRegisterImplementations register)
        {
            //register.RegisterInstance<ICurrentDateTime>(Clock);
        }

        protected IStore<TComponent> Store<TComponent>()
        {
            VerifyComponentTypeIsRegistered<TComponent>();
            return Build<IStore<TComponent>>();
        }

        protected void Assign<TComponent>(Guid entity, TComponent component)
        {
            VerifyComponentTypeIsRegistered<TComponent>();
            Store<TComponent>().AssignCommit(entity, component);
        }

        protected Guid Add<TComponent>(TComponent newComponent)
        {
            VerifyComponentTypeIsRegistered<TComponent>();
            return Store<TComponent>().AddCommit(newComponent);
        }

        private void VerifyComponentTypeIsRegistered<TComponent>()
        {
            if (!Catalog.Contains<TComponent>())
            {
                throw new InvalidOperationException(
                    string.Format("Type '{0}' is not registered as a component within this test class.", typeof(TComponent)));
            }
        }

        protected Lazy<T> Lazy<T>(Func<T> factory)
        {
            return new Lazy<T>(factory);
        }

        protected T Build<T>()
        {
            T result = Container.Build<T>();
            if (result == null)
            {
                throw new BuildException(string.Format("Could not build type '{0}' or one of its dependencies.", typeof(T)));
            }

            return result;
        }

        protected IQueryPlan<T> Query<T>()
        {
            return Build<IQueryPlan<T>>();
        }

        protected void StartQueryCounter()
        {
            ExecutedQueryCount = 0;
            if (Db != null)
            {
                Db.Database.Log = QueryExecuted;
            }
        }

        protected void StopQueryCounter()
        {
            if (Db != null)
            {
                Db.Database.Log = null;
            }
        }

        private void QueryExecuted(string sql)
        {
            if (sql.StartsWith("-- Executing at "))
            {
                ExecutedQueryCount++;
            }
        }

        protected void AddComponentPackage(IComponentPackage package)
        {
            ComponentPackages.Add(package);
            RegisterPackages();
        }

        //protected TScenario UseScenario<TScenario>()
        //    where TScenario : IScenario
        //{
        //    var scenario = Build<TScenario>();

        //    AddComponentPackage(scenario);

        //    Scenarios.Add(scenario);

        //    scenario.Seed(new ComponentSeeder(Catalog));

        //    return scenario;
        //}

        //protected void Reseed()
        //{
        //    Catalog.PermanentlyDestroyAllData();

        //    var seeder = new ComponentSeeder(Catalog);
        //    Scenarios.ForEach(o => o.Seed(seeder));
        //}

        public void RegisterFactoryMethod(Type requestedType, Func<IBuildSession, object> factoryMethod)
        {
            BuildStrategies.RegisterFactoryMethod(requestedType, factoryMethod);
        }

        protected void AssertThrows<TException>(Action action)
            where TException : Exception
        {
            bool thrown = false;
            try
            {
                action();
            }
            catch (TException)
            {
                thrown = true;
            }

            if (!thrown)
            {
                throw new AssertFailedException($"An exception of type {typeof(TException)} was expected, but not thrown.");
            }
        }
    }
}
