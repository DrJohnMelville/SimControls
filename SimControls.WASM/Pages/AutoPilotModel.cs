using Melville.INPC;
using SimControls.Model;
using SimControls.Model.CompositeElements;

namespace SimControls.WASM.Pages
{
    [MacroItem("SimulationRate", "SingleStepDoubleItem")]
    [MacroItem("AutopilotHeading", "TwoStepDoubleItem")]
    [MacroItem("AutopilotAltitudeLock", "TwoStepDoubleItem")]
    [MacroItem("AutopilotVerticalSpeed", "TwoStepDoubleItem")]
    [MacroItem("AutopilotAirSpeed", "TwoStepDoubleItem")]
    [MacroItem("AutopilotAltitudeHold", "IBoolItem")]
    [MacroItem("AutopilotVsiHold", "IBoolItem")]
    [MacroItem("AutopilotAirspeedHold", "IBoolItem")]
    [MacroItem("AutopilotWingLeveler", "IBoolItem")]
    [MacroItem("AutopilotHeadingLock", "IBoolItem")]
    [MacroItem("AutopilotNav1Lock", "IBoolItem")]
    [MacroCode("public ~1~ ~0~ {get;}")]
    [MacroCode(@"    ~0~ = variables.~0~Element(); RegisterPropertyChangeForwarding(~0~);", 
        Prefix = "public AutoPilotModel(IVariableCache variables) {",
        Postfix = "}")]
    public partial class AutoPilotModel: ForwardPropertyChanged
    {
    }
}