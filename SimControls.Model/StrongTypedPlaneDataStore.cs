using System;
using Melville.INPC;
using SimControls.Model.CompositeElements;

namespace SimControls.Model
{
    public static partial class StrongTypedPlaneDataStore
    {
        public static BoundedDoubleItem FlapsElement(this IVariableCache vc) =>
            new (vc.FlapsHandleIndex(), ConstantDataItem.FromValue(0.0), vc.FlapsNumHandlePositions());

        private const double DegreesToRadians = Math.PI / 180;
        public static BoundedDoubleItem ElevatorTrimElement(this IVariableCache vc) =>
            new (vc.ElevatorTrimPosition(),
                ConstantDataItem.FromValue(-20 * DegreesToRadians),
                ConstantDataItem.FromValue(20 * DegreesToRadians));
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