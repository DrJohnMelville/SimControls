using Melville.INPC;
using SimControls.Model.CompositeElements;

namespace SimControls.Model
{
    public static partial class StrongTypedPlaneDataStore
    {
    }

    [MacroItem("ParkingBreakElement", "BrakeParkingIndicator", "ParkingBrakesEvent")]
    [MacroItem("GearDownElement", "GearHandlePosition", "GearToggleEvent")]
    [MacroItem("AutopilotElement", "AutopilotMaster", "ApMasterEvent")]
    [MacroItem("AutopilotAltitudeHoldElement", "AutopilotAltitudeLock", "ApPanelAltitudeHoldEvent")]
    [MacroItem("AutopilotVsiHoldElement", "AutopilotVerticalHold", "ApVsHoldEvent")]
    [MacroItem("AutopilotAirspeedHoldElement", "AutopilotAirspeedHold", "ApPanelSpeedHoldEvent")]
    [MacroItem("AutoPilotWingLevelerElement","AutopilotWingLeveler","ApWingLevelerEvent")]
    [MacroItem("AutopilotHeadingLockElement","AutopilotHeadingLock", "ApPanelHeadingHoldEvent")]
    [MacroItem("AutoPilotNav1LockElement","AutopilotNav1Lock","ApNav1HoldEvent")]
    [MacroCode(@" public static IBoolItem ~0~(this IVariableCache vc) =>
    new BoolWithToggleElement(vc.~1~(), vc.~2~());")]
    public static partial class BooleanPlaneElements
    {
    }
}