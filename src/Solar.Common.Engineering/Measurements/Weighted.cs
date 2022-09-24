using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public static class Weighted
    {
        /// <summary>
        /// Calculates a weighted average of values, based on the <paramref name="valueSelect"/> function, from a set of weighted items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weightedItems"></param>
        /// <param name="valueSelect"></param>
        /// <returns></returns>
        public static double WeightedAverage<T>(this IEnumerable<Weighted<T>> weightedItems, Func<T, double> valueSelect)
        {
            return weightedItems.Sum(o => valueSelect(o.Model) * o.Weight);
        }

        public static Weighted<T> From<T>(T model, double weight)
        {
            return new Weighted<T>(model, weight);
        }
    }

    public class Weighted<T>
    {
        public T Model { get; private set; }
        public double Weight { get; private set; }

        public Weighted(T model, double weight)
        {
            this.Model = model;
            this.Weight = weight;
        }
    }
}
