using SolarEcs.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEcs.Tests.EntityFramework.Infrastructure
{
    /// <summary>
    /// Component types used to exercise the EF provider. All of these are registered
    /// through a single package so that every EF-backed test class gets an identical
    /// EF model (EF6 caches the compiled model per context type within an AppDomain).
    /// </summary>
    public class TextComponent
    {
        public string Text { get; set; }
    }

    public class RichComponent
    {
        public string Text { get; set; }
        public double Number { get; set; }
        public int? OptionalNumber { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? Timestamp { get; set; }
        public decimal? Amount { get; set; }
        public Guid? RelatedId { get; set; }
    }

    /// <summary>
    /// A nested complex type used to exercise complex-type flattening
    /// (column names like "Money_Currency" / "Money_Amount").
    /// </summary>
    public class Money
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public class MoneyAndLabelComponent
    {
        public Money Money { get; set; }
        public string Label { get; set; }
    }

    public class BlobComponent
    {
        public byte[] Data { get; set; }
    }

    public class TestComponentPackage : IComponentPackage
    {
        public void Register(IComponentRegistration register)
        {
            register.Component<TextComponent>();
            register.Component<RichComponent>();
            register.Component<MoneyAndLabelComponent>();
            register.Component<BlobComponent>();

            register.ValueType<Money>();
        }
    }
}
