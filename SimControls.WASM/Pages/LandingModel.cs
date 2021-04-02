using Melville.INPC;
using SimControls.Model;
using SimControls.Model.CompositeElements;

namespace SimControls.WASM.Pages
{
    public partial class LandingModel: ForwardPropertyChanged
    {
        public BoundedDoubleItem ElevatorTrim { get; }
        public BoundedDoubleItem Flaps { get; }
        
        [MacroItem("GearDown", "GearDownElement")]
        [MacroItem("ParkingBrake", "ParkingBreakElement")]
        [MacroItem("LandingLights", "LandingLightElement")]
        [MacroItem("CabinLights", "CabinLightElement")]
        [MacroItem("BeaconLights", "BeaconLightElement")]
        [MacroItem("LogoLights", "LogoLightElement")]
        [MacroItem("NavLights", "NavLightElement")]
        [MacroItem("PanelLights", "PanelLightElement")]
        [MacroItem("RecognitionLights", "RecognitionLightElement")]
        [MacroItem("StrobeLights", "StrobeLightElement")]
        [MacroItem("TaxiLights", "TaxiLightElement")]
        [MacroItem("WingLights", "WingLightElement")]
        [MacroCode("public IBoolItem ~0~ {get; private set;} = null!;")]
        [MacroCode("    ~0~ = variables.~1~(); RegisterPropertyChangeForwarding(~0~);", 
            Prefix = "private void InnerAssignModel(IVariableCache variables) {",
            Postfix = "}")]
        public IReadOnlyBoolItem GearRetractable { get; }
        
        public LandingModel(IVariableCache variables)
        {
            ElevatorTrim = variables.ElevatorTrimElement();
            Flaps = variables.FlapsElement();
            GearRetractable = variables.IsGearRetractable();
            InnerAssignModel(variables);

            RegisterPropertyChangeForwarding(ElevatorTrim, Flaps, GearRetractable);
        }
    }
}