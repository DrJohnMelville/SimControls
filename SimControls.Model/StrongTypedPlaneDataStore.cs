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

    public static partial class DoubleOffsetPlaneElements
    {
        [MacroItem("AutopilotHeadingElement", "AutopilotHeadingLockDir", "HeadingBugSetEvent", 0,360, 1, 15,"Rolling")]
        [MacroItem("AutopilotAltitudeLockElement", "AutopilotAltitudeLockVar", "ApAltVarSetEnglishEvent", 0,1_000_000, 100, 1000,"Clamping")]
        [MacroItem("AutopilotVerticalSpeedElement", "AutopilotVerticalHoldVar", "ApVsVarSetEnglishEvent", -2000,2000, 10, 100, "Clamping")]
        [MacroItem("AutopilotAirSpeedElement", "AutopilotAirspeedHoldVar", "ApSpdVarSetEvent", 0,1400, 1, 10, "Clamping")]
        [MacroCode(@"        public static TwoStepDoubleItem ~0~(this IVariableCache vc) =>
            new(
                new DoubleWithSetEvent(vc.~1~(), vc.~2~()),
                ConstantDataItem.FromValue(~3~.0), ConstantDataItem.FromValue(~4~.0),
                ConstantDataItem.FromValue(~5~.0), ConstantDataItem.FromValue(~6~.0),
                BoundedDoubleItem.~7~);
")]
        public static LogrithmicStepDoubleItem SimulationRateElement(this IVariableCache variables) =>
            new(new DoubleWithSetEvent(variables.SimulationRate(), variables.SimRateEvent()),
                new ConstantDataItem<double>(0.25), new ConstantDataItem<double>(16), new ConstantDataItem<double>(2));
    }

    //Gear and parking
    [MacroItem("ParkingBreakElement", "BrakeParkingIndicator", "ParkingBrakesEvent")]
    [MacroItem("GearDownElement", "GearHandlePosition", "GearToggleEvent")]
    //AutoPilot
    [MacroItem("AutopilotElement", "AutopilotMaster", "ApMasterEvent")]
    [MacroItem("AutopilotAltitudeHoldElement", "AutopilotAltitudeLock", "ApPanelAltitudeHoldEvent")]
    [MacroItem("AutopilotVsiHoldElement", "AutopilotVerticalHold", "ApVsHoldEvent")]
    [MacroItem("AutopilotAirspeedHoldElement", "AutopilotAirspeedHold", "ApPanelSpeedHoldEvent")]
    [MacroItem("AutopilotWingLevelerElement","AutopilotWingLeveler","ApWingLevelerEvent")]
    [MacroItem("AutopilotHeadingLockElement","AutopilotHeadingLock", "ApPanelHeadingHoldEvent")]
    [MacroItem("AutopilotNav1LockElement","AutopilotNav1Lock","ApNav1HoldEvent")]
    //Lights
    [MacroItem("LandingLightElement", "LightLanding","LandingLightsToggleEvent")]
    [MacroItem("CabinLightElement", "LightCabin","ToggleCabinLightsEvent")]
    [MacroItem("BeaconLightElement", "LightBeacon","ToggleBeaconLightsEvent")]
    [MacroItem("LogoLightElement", "LightLogo","ToggleLogoLightsEvent")]
    [MacroItem("NavLightElement", "LightNav","ToggleNavLightsEvent")]
    [MacroItem("PanelLightElement", "LightPanel","PanelLightsToggleEvent")]
    [MacroItem("RecognitionLightElement", "LightRecognition","ToggleRecognitionLightsEvent")]
    [MacroItem("StrobeLightElement", "LightStrobe","StrobesToggleEvent")]
    [MacroItem("TaxiLightElement", "LightTaxi","ToggleTaxiLightsEvent")]
    [MacroItem("WingLightElement", "LightWing","ToggleWingLightsEvent")]
    [MacroCode(@" public static IBoolItem ~0~(this IVariableCache vc) =>
    new BoolWithToggleElement(vc.~1~(), vc.~2~());")]
    public static partial class BooleanPlaneElements
    {
    }
}