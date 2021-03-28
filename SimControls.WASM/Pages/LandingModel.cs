using SimControls.Model;
using SimControls.Model.CompositeElements;

namespace SimControls.WASM.Pages
{
    public class LandingModel: ForwardPropertyChanged
    {
        public BoundedDoubleItem ElevatorTrim { get; }
        public BoundedDoubleItem Flaps { get; }
        public IReadOnlyBoolItem GearRetractable { get; }
        public IBoolItem GearDown { get; }
        
        public LandingModel(IVariableCache variables)
        {
            ElevatorTrim = variables.ElevatorTrimElement();
            Flaps = variables.FlapsElement();
            GearRetractable = variables.IsGearRetractable();
            GearDown = variables.GearDownElement();
            RegisterPropertyChangeForwarding(ElevatorTrim, Flaps, GearDown, GearRetractable);
        }
    }
}