using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Melville.MVVM.BusinessObjects;
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
            var storeElevatorTrim = store.ElevatorTrimPosition();
            ElevatorTrim = new SliderModel(storeElevatorTrim,
                20*degreesToRadians,-20*degreesToRadians, Orientation.Vertical).WithLabel("Elevator");
            
            Flaps = new ConstrainedSliderModel(
                store.FlapsHandleIndex(),
                store.FlapsNumHandlePositions(), Orientation.Vertical, true)
                .WithLabel("Flaps");
            ParkingBreak = new ToggleButtonModel("Parking Break", store.ParkingBreakElement());
            GearRetractable = store.IsGearRetractable();
            GearDown = new ToggleButtonModel("Landing Gear", store.GearDownElement());
            
            SimulationRate = new DoubleUpDownDisplayModel(store.SimulationRate(),
                    store.SimRateDecrEvent(), store.SimRateIncrEvent(),
                    store.SimRateEvent())
                .WithLabel("Time Factor", Dock.Left);

            AutoPilotActive = new ToggleButtonModel("Auto Pilot Active", store.AutopilotElement()); 
            AutoPilotAvialable = store.AutopilotAvailable();
            HeadingBug = new DoubleUpDownDisplayModel(store.AutopilotHeadingLockDir(),
                store.HeadingBugDecEvent(), store.HeadingBugIncEvent(), store.HeadingBugSetEvent());
            AltitudeBug = new DoubleUpDownDisplayModel(store.AutopilotAltitudeLockVar(),
                store.ApAltVarSetEnglishEvent(), 100);
            VSIBug = new DoubleUpDownDisplayModel(store.AutopilotVerticalHoldVar(),
                store.ApVsVarSetEnglishEvent(), 100);
            AirspeedBug = new DoubleUpDownDisplayModel(store.AutopilotAirspeedHoldVar(),
                store.ApSpdVarSetEvent(), 5);

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

    public class DoubleUpDownDisplayModel: NotifyBase, IDisplayModel
    {
        private readonly ReadOnlyDataItem<double> item;
        private readonly Action down;
        private readonly Action up;
        private readonly SimEventTrigger? setter;

        public DoubleUpDownDisplayModel(
            ReadOnlyDataItem<double> item, SimEventTrigger? setter, double increment): 
            this(item, null, null, setter, increment) {}
        
        public DoubleUpDownDisplayModel(
            ReadOnlyDataItem<double> item, SimEventTrigger? down, 
            SimEventTrigger? up, SimEventTrigger? setter, double increment = 1)
        {
            this.item = item;
            this.setter = setter;
            this.up = up == null ? () => Value += increment: ()=>up.Fire();
            this.down = down == null ? () => Value -= increment: ()=>down.Fire();
            DelegatePropertyChangeFrom(item, nameof(item.Value), nameof(Value));
        }

        public double Value
        {
            get => Math.Round(item.Value);
            set => setter?.Fire((uint)value);
        }

        public void BumpDown() => down();
        public void BumpUp() => up();
    }

    public class DisplayModel<T>: IDisplayModel
    {
        public DataItem<T> Item { get; }
        public DisplayModel(DataItem<T> item)
        {
            Item = item;
        }
    }
}