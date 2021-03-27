using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Melville.MVVM.Wpf.Bindings;
using SimControls.Model;
using Orientation = System.Windows.Controls.Orientation;

namespace SimControls.FlightDisplays
{
    public class FlightDisplayViewModel
    {
        public IDisplayModel ElevatorTrim { get; }
        public IDisplayModel Flaps { get; }
        public IDisplayModel ParkingBreak { get; }
        public IDisplayModel GearDown { get; }
        public IReadOnlyBoolItem GearRetractable { get; }
        
        public IDisplayModel SimulationRate { get; }
        private const double degreesToRadians = Math.PI / 180.0;
        
        public IReadOnlyBoolItem AutoPilotAvialable { get; }
        public IDisplayModel AutoPilotActive { get; }
        public IDisplayModel HeadingBug { get; }
        public IDisplayModel AltitudeBug { get; }
        public IDisplayModel VSIBug { get; }
        public IDisplayModel AltitudeHold { get; }
        public IDisplayModel VSIHold { get; }
        public IDisplayModel AirspeedBug { get; }
        public IDisplayModel AirspeedHold { get; }
        
        public IDisplayModel WingLeveler { get; }
        public IDisplayModel HeadingMode { get; }
        public IDisplayModel NavMode { get; }
        public FlightDisplayViewModel(IVariableCache store)
        {
            ElevatorTrim = new SliderModel(store.ElevatorTrimElement()).WithLabel("Elevator");
            
            Flaps = new ConstrainedSliderModel(store.FlapsElement(), Orientation.Vertical, true)
                .WithLabel("Flaps");
            ParkingBreak = new ToggleButtonModel("Parking Break", store.ParkingBreakElement());
            GearRetractable = store.IsGearRetractable();
            GearDown = new ToggleButtonModel("Landing Gear", store.GearDownElement());
            
            SimulationRate = new DoubleUpDownDisplayModel(store.SimulationRateElement())
                .WithLabel("Time Factor", Dock.Left); 

            AutoPilotActive = new ToggleButtonModel("Auto Pilot Active", store.AutopilotElement()); 
            AutoPilotAvialable = store.AutopilotAvailable();
            HeadingBug = new DoubleUpDownDisplayModel(store.AutopilotHeadingElement());
            AltitudeBug = new DoubleUpDownDisplayModel(store.AutopilotAltitudeLockElement());
            VSIBug = new DoubleUpDownDisplayModel(store.AutopilotVerticalSpeedElement());
            AirspeedBug = new DoubleUpDownDisplayModel(store.AutopilotAirSpeedElement());

            AltitudeHold = new ToggleButtonModel("Alt Hold", store.AutopilotAltitudeHoldElement());
            VSIHold = new ToggleButtonModel("VSI Hold", store.AutopilotVsiHoldElement());
            AirspeedHold = new ToggleButtonModel("Airspeed Hold", store.AutopilotAirspeedHoldElement());

            WingLeveler = new ToggleButtonModel("Wings Level", store.AutoPilotWingLevelerElement());
            HeadingMode = new ToggleButtonModel("Hdg Lock", store.AutopilotHeadingLockElement());
            NavMode = new ToggleButtonModel("Nav Lock", store.AutoPilotNav1LockElement());
        }
    }

    public static class IDisplayModelOperations
    {
        public static IDisplayModel WithLabel(this IDisplayModel model, string label, Dock location = Dock.Top) =>
          new LabeledDisplayModel(model, label, location);
    }
    public interface IDisplayModel{}
}