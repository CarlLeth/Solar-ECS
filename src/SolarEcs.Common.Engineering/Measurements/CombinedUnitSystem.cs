using SolarEcs.Common.Identification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Common.Engineering.Measurements
{
    public class CombinedUnitSystem : ICombinedUnitSystem
    {
        private readonly IStore<UnitConversionCategorization> Categorizations;
        private readonly IStore<ScalarUnitOfMeasure> ScalarUnits;
        private readonly IStore<ReferenceUnitOfMeasure> ReferenceUnits;
        private readonly INameSystem Names;
        private readonly ITextSystem Descriptions;
        private readonly IStore<Abbreviation> Abbreviations;

        public CombinedUnitSystem(IStore<UnitConversionCategorization> categorizations, IStore<ScalarUnitOfMeasure> scalarUnits, IStore<ReferenceUnitOfMeasure> referenceUnits,
            INameSystem names, ITextSystem descriptions, IStore<Abbreviation> abbreviations)
        {
            Categorizations = categorizations;
            ScalarUnits = scalarUnits;
            ReferenceUnits = referenceUnits;
            Names = names;
            Descriptions = descriptions;
            Abbreviations = abbreviations;
        }

        public IQueryPlan<CombinedUnitModel> Query
        {
            get
            {
                return Categorizations.ToQueryPlan()
                    .EntityLeftJoin(ScalarUnits.ToQueryPlan())
                    .EntityLeftJoin(ReferenceUnits.ToQueryPlan())
                    .EntityLeftJoin(Names.Query)
                    .EntityLeftJoin(Descriptions.Query)
                    .EntityLeftJoin(Abbreviations.ToQueryPlan())
                    .ResolveJoin((cat, scalar, reference, name, desc, abbrev) => new CombinedUnitModel(
                        cat.Model.UnitConversionCategory,
                        name.Get(o => o.Name),
                        abbrev.Get(o => o.Text),
                        desc.Get(o => o.Text),
                        reference.Get<double?>(o => o.ConversionFactor, scalar.Get(o => o.ConversionFactor)),
                        reference.Get<double?>(o => o.ZeroPoint)
                    ));
            }
        }

        public IRecipe<CombinedUnitModel> Recipe
        {
            get
            {
                return Query.StartRecipe()
                    .IncludeSimple(Categorizations.ToRecipe(), o => new UnitConversionCategorization(o.ConversionCategory))
                    .IncludeSimple(ScalarUnits.ToRecipe(), o => new ScalarUnitOfMeasure(o.ConversionFactor.Value), o => o.ConversionFactor.HasValue && (!o.ZeroPoint.HasValue || o.ZeroPoint.Value == 0))
                    .IncludeSimple(ReferenceUnits.ToRecipe(), o => new ReferenceUnitOfMeasure(o.ConversionFactor.Value, o.ZeroPoint.Value), o => o.ConversionFactor.HasValue && o.ZeroPoint.HasValue && o.ZeroPoint.Value != 0)
                    .IncludeSimple(Names.Recipe, o => new NameModel(o.Name), o => !string.IsNullOrEmpty(o.Name))
                    .IncludeSimple(Descriptions.Recipe, o => new TextModel(o.Description), o => !string.IsNullOrEmpty(o.Description))
                    .IncludeSimple(Abbreviations.ToRecipe(), o => new Abbreviation(o.Abbreviation), o => !string.IsNullOrEmpty(o.Abbreviation));
            }
        }
    }
}
