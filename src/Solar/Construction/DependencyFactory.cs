using Fusic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Ecs.Construction
{
    public class DependencyFactory<TDependency> : IDependencyFactory<TDependency>
    {
        private IFusicContainer Container { get; set; }

        public DependencyFactory(IFusicContainer container)
        {
            this.Container = container;
        }

        public TDependency Build()
        {
            return Container.Build<TDependency>();
        }
    }
}
