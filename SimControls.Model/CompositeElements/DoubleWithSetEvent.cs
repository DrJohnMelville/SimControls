using System.ComponentModel;
using Melville.INPC;

namespace SimControls.Model.CompositeElements
{
    public partial class DoubleWithSetEvent : IDataItem<double>
    {
        [DelegateTo]
        private readonly IReadOnlyDataItem<double> source;
        private readonly ISimEventTrigger setter;
        
        public DoubleWithSetEvent(IReadOnlyDataItem<double> source, ISimEventTrigger setter)
        {
            this.source = source;
            this.setter = setter;
        }

        public double Value
        {
            get => source.Value;
            set => setter.Fire((uint) value);
        }
    }
}