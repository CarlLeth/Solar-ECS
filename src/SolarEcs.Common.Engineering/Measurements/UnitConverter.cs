using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class UnitConverter : IUnitConverter
    {
        private readonly IUnitConversionSystem UnitConversionSystem;
        private readonly IStore<UnitConversionCategorization> Categorizations;

        private Dictionary<Guid, IUnitConversionStrategy> StrategiesByUnit;
        private Dictionary<Guid, Guid> CategoriesByUnit;

        public UnitConverter(IUnitConversionSystem unitConversionSystem, IStore<UnitConversionCategorization> categorizations)
        {
            this.UnitConversionSystem = unitConversionSystem;
            this.Categorizations = categorizations;
        }

        public double Normalize(Measurement measurement)
        {
            measurement.EnsureComplete("Cannot normalize an incomplete measurement.");

            return Get(measurement.UnitOfMeasure.Value).Normalize(measurement.Value.Value);
        }

        public Measurement Denormalize(double normalValue, Guid targetUnitOfMeasure)
        {
            return new Measurement(Get(targetUnitOfMeasure).Denormalize(normalValue), targetUnitOfMeasure);
        }

        public double Scale(double measuredValue, Guid byUnitOfMeasure, double exponent)
        {
            return Get(byUnitOfMeasure).Scale(measuredValue, exponent);
        }

        private IUnitConversionStrategy Get(Guid unitOfMeasure)
        {
            if (StrategiesByUnit == null)
            {
                StrategiesByUnit = UnitConversionSystem.Strategies.ExecuteAll().ToDictionary(o => o.Key, o => o.Model);
            }

            if (!StrategiesByUnit.ContainsKey(unitOfMeasure))
            {
                throw new InvalidOperationException($"No unit conversion strategy exists for unit '{unitOfMeasure}'.");
            }

            return StrategiesByUnit[unitOfMeasure];
        }

        public Guid GetConversionCategory(Guid unitOfMeasure)
        {
            if (CategoriesByUnit == null)
            {
                CategoriesByUnit = Categorizations.All.ToDictionary(o => o.Id, o => o.Component.UnitConversionCategory);
            }

            if (!CategoriesByUnit.ContainsKey(unitOfMeasure))
            {
                throw new InvalidOperationException($"No unit conversion category exists for unit '{unitOfMeasure}'.");
            }

            return CategoriesByUnit[unitOfMeasure];
        }
    }
}
