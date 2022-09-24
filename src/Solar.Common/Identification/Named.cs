using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar.Common.Identification
{
    public static class Named
    {
        public static Named<TModel> From<TModel>(string name, TModel model)
        {
            return new Named<TModel>(name, model);
        }
    }

    public class Named<TModel>
    {
        public string Name { get; private set; }
        public TModel Model { get; private set; }

        public Named(string name, TModel model)
        {
            this.Name = name;
            this.Model = model;
        }

        private Named() { }

        public Named<TProjected> Select<TProjected>(Func<TModel, TProjected> projection)
        {
            return Named.From(Name, projection(Model));
        }
    }
}
