using System;
using Melville.INPC;

namespace SimControls.Model.CompositeElements
{
    public delegate double DoubleClampStratey(BoundedDoubleItem item, double value);
    public partial class BoundedDoubleItem : IDataItem<double>
    {
        [DelegateTo()]
        private readonly IDataItem<double> source;
        private IReadOnlyDataItem<double> Minimum {get;}
        private IReadOnlyDataItem<double> Maximum {get;}
        private readonly DoubleClampStratey clampStrategy;

        public BoundedDoubleItem(
            IDataItem<double> source, 
            IReadOnlyDataItem<double> minimum, 
            IReadOnlyDataItem<double> maximum, 
            DoubleClampStratey? clampStrategy = null)
        {
            this.source = source;
            Minimum = minimum;
            Maximum = maximum;
            this.clampStrategy = clampStrategy ?? Clamping;
        }
        public double Value
        {
            get => source.Value;
            set => source.Value = clampStrategy(this, value);
        }

        public static double Clamping(BoundedDoubleItem item, double value) =>
            Math.Clamp(value, item.Minimum.Value, item.Maximum.Value);

        public static double Rolling(BoundedDoubleItem item, double value)
        {
            var delta = item.Maximum.Value - item.Minimum.Value;
            while (value < item.Minimum.Value) value += delta;
            while (value >= item.Maximum.Value) value -= delta;
            return value;
    }
}