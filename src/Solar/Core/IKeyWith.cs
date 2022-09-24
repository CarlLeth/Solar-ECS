using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solar
{
    public interface IKeyWith<out TKey, out TModel>
    {
        TKey Key { get; }
        TModel Model { get; }
    }

    public class KeyWith<TKey, TModel> : IKeyWith<TKey, TModel>
    {
        public TKey Key { get; set; }
        public TModel Model { get; set; }

        public KeyWith(TKey key, TModel model)
        {
            this.Key = key;
            this.Model = model;
        }

        public KeyWith() { }
    }

    public static class KeyWith
    {
        public static KeyWith<TKey, TModel> Create<TKey, TModel>(TKey key, TModel model)
        {
            return new KeyWith<TKey, TModel>(key, model);
        }
    }
}
