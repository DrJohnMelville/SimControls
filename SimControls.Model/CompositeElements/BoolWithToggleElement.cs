using Melville.INPC;

namespace SimControls.Model.CompositeElements
{
    public partial class BoolWithToggleElement: IBoolItem
    {
        [DelegateTo()]
        private readonly IReadOnlyBoolItem inner;
        private readonly ISimEventTrigger trigger;
        
        public BoolWithToggleElement(IReadOnlyBoolItem inner, ISimEventTrigger trigger)
        {
            this.inner = inner;
            this.trigger = trigger;
        }

        public bool BoolValue
        {
            get => inner.BoolValue;
            set
            {
                if (BoolValue != value) trigger.Fire();
            }
        }
    }
}