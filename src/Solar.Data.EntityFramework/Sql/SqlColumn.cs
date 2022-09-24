using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Data.EntityFramework.Sql
{
    public class SqlColumn<TComponent>
    {
        public string Name { get; }
        public Type PropertyType { get; }
        public Func<TComponent, object> Getter { get; }

        public SqlColumn(string name, Type propertyType, Func<TComponent, object> getter)
        {
            this.Name = name;
            this.PropertyType = propertyType;
            this.Getter = getter;
        }
    }
}
