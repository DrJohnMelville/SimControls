using System;
using Melville.INPC;

namespace SimControls.Model.CompositeElements
{
    public delegate double DoubleClampStratey(BoundedDoubleItem item, double value);

    public partial class BoundedDoubleItem : IDataItem<double>
    {
        [DelegateTo()] private readonly IDataItem<double> source;
        public IReadOnlyDataItem<double> Minimum { get; }
        public IReadOnlyDataItem<double> Maximum { get; }
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

        public double InvertedValue
        {
            get =>  InvertValue(Value);
            set => Value = InvertValue(value);
        }

        private double InvertValue(double value) => Maximum.Value - (value - Minimum.Value);

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
    
    public class SingleStepDoubleItem: BoundedDoubleItem
    {
        public IReadOnlyDataItem<double> StepSize { get; }
        public SingleStepDoubleItem(
            IDataItem<double> source, 
            IReadOnlyDataItem<double> minimum, 
            IReadOnlyDataItem<double> maximum, 
            IReadOnlyDataItem<double> stepSize, 
            DoubleClampStratey? clampStrategy = null) : 
            base(source, minimum, maximum, clampStrategy)
        {
            StepSize = stepSize;
        }

        public virtual void Increment() => Value += StepSize.Value;
        public virtual void Decrement() => Value -= StepSize.Value;
    }

    public class LogrithmicStepDoubleItem : SingleStepDoubleItem
    {
        public LogrithmicStepDoubleItem(
            IDataItem<double> source, 
            IReadOnlyDataItem<double> minimum, 
            IReadOnlyDataItem<double> maximum, 
            IReadOnlyDataItem<double> stepSize, 
            DoubleClampStratey? clampStrategy = null) 
            : base(source, minimum, maximum, stepSize, clampStrategy)
        {
        }

        public override void Increment() => Value *= StepSize.Value;
        public override void Decrement() => Value /= StepSize.Value;
    }

    public class TwoStepDoubleItem : SingleStepDoubleItem
    {
        public IReadOnlyDataItem<double> BigStepSize { get; }
        public TwoStepDoubleItem(
            IDataItem<double> source, 
            IReadOnlyDataItem<double> minimum, 
            IReadOnlyDataItem<double> maximum, 
            IReadOnlyDataItem<double> stepSize, 
            IReadOnlyDataItem<double> bigStepSize, 
            DoubleClampStratey? clampStrategy = null) : 
            base(source, minimum, maximum, stepSize, clampStrategy)
        {
            BigStepSize = bigStepSize;
        }
        public void BigIncrement() => Value += BigStepSize.Value;
        public void BigDecrement() => Value -= BigStepSize.Value;
    }
}