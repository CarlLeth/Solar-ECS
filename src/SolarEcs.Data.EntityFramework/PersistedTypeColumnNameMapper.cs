using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework
{
    /// <summary>
    /// Configures every property of the Component associated with a persisted type to remove "Component_" from the name of the mapped column.
    /// </summary>
    /// <typeparam name="TPersisted"></typeparam>
    public class PersistedTypeColumnNameMapper<TPersisted>
        where TPersisted : class
    {
        private EntityTypeConfiguration<TPersisted> Config { get; set; }

        private Type ComponentType { get; set; }
        private ParameterExpression Parameter { get; set; }
        private Expression ComponentAccess { get; set; }
        
        private MethodInfo SpecifyColumnNameStructMethod { get; set; }
        private MethodInfo SpecifyColumnNameNullableMethod { get; set; }

        private Dictionary<Type, Action<Expression, string>> SpecificTypeMappers { get; set; }

        public PersistedTypeColumnNameMapper(DbModelBuilder modelBuilder, Type componentType)
        {
            this.Config = modelBuilder.Entity<TPersisted>();
            this.ComponentType = componentType;
            this.Parameter = Expression.Parameter(typeof(TPersisted));
            this.ComponentAccess = Expression.Property(Parameter, "Component");

            SpecifyColumnNameStructMethod = this.GetType().GetMethod(nameof(SpecifyPropertyColumnNameStruct), BindingFlags.NonPublic | BindingFlags.Instance);
            SpecifyColumnNameNullableMethod = this.GetType().GetMethod(nameof(SpecifyPropertyColumnNameNullable), BindingFlags.NonPublic | BindingFlags.Instance);

            InitializeSpecificTypeMappers();
        }

        private void InitializeSpecificTypeMappers()
        {
            SpecificTypeMappers = new Dictionary<Type, Action<Expression, string>>();

            // Select the correct overloaded message for these types specifically handled by Entity Framework
            SpecificTypeMappers.Add(typeof(byte[]), (expr, name) => Config.Property((Expression<Func<TPersisted, byte[]>>)expr).HasColumnName(name));
            SpecificTypeMappers.Add(typeof(string), (expr, name) => Config.Property((Expression<Func<TPersisted, string>>)expr).HasColumnName(name));
            SpecificTypeMappers.Add(typeof(System.Data.Entity.Spatial.DbGeography), (expr, name) => Config.Property((Expression<Func<TPersisted, DbGeography>>)expr).HasColumnName(name));
            SpecificTypeMappers.Add(typeof(System.Data.Entity.Spatial.DbGeometry), (expr, name) => Config.Property((Expression<Func<TPersisted, DbGeometry>>)expr).HasColumnName(name));
        }

        public void Map()
        {
            MapProperty(ComponentType, ComponentAccess, null);
        }

        private void MapProperty(Type propertyType, Expression baseExpression, string name)
        {
            if (SpecificTypeMappers.ContainsKey(propertyType))
            {
                SpecificTypeMappers[propertyType].Invoke(Expression.Lambda(baseExpression, Parameter), name);
            }
            else if (TypeIsComplex(propertyType))
            {
                MapComplexType(propertyType, baseExpression, name);
            }
            else
            {
                MapValueType(propertyType, baseExpression, name);
            }
        }

        private bool TypeIsComplex(Type type)
        {
            return !type.IsValueType && (!type.IsGenericType || type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        private void MapComplexType(Type complexType, Expression baseExpression, string name)
        {
            foreach (var property in complexType.GetProperties())
            {
                var expression = Expression.Property(baseExpression, property);

                if (property.CanWrite)
                {
                    MapProperty(
                        property.PropertyType,
                        expression,
                        name == null ? property.Name : string.Format("{0}_{1}", name, property.Name)
                    );
                }
            }
        }

        private void MapValueType(Type propertyType, Expression expression, string name)
        {
            var propertyAccessor = Expression.Lambda(expression, Parameter);

            MethodInfo specifyMethod;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var innerType = propertyType.GetGenericArguments()[0];
                specifyMethod = SpecifyColumnNameNullableMethod.MakeGenericMethod(innerType);
            }
            else
            {
                specifyMethod = SpecifyColumnNameStructMethod.MakeGenericMethod(propertyType);
            }

            specifyMethod.Invoke(this, new object[] { propertyAccessor, name });
        }

        private void SpecifyPropertyColumnNameStruct<TProperty>(Expression<Func<TPersisted, TProperty>> propertyAccessor, string columnName)
            where TProperty : struct
        {
            Config.Property(propertyAccessor).HasColumnName(columnName);
        }

        private void SpecifyPropertyColumnNameNullable<TProperty>(Expression<Func<TPersisted, TProperty?>> propertyAccessor, string columnName)
            where TProperty : struct
        {
            Config.Property(propertyAccessor).HasColumnName(columnName);
        }
    }
}
