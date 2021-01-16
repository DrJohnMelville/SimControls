using System.ComponentModel;
using System.Runtime.CompilerServices;
using SimControls.Model;

namespace SimControls.FlightDisplays
{
    public class ToggleButtonModel : IDisplayModel, INotifyPropertyChanged
    {
        public string Title { get; }
        private readonly IReadOnlyBoolItem item;
        private readonly SimEventTrigger? simEvent;

        public bool Value
        {
            get => item.BoolValue;
            set
            {
                if (item.BoolValue == value) return;
                //if (item is BoolItem bi) bi.BoolValue = value;
                simEvent?.Fire();
            }
        }

        public ToggleButtonModel(BoolItem item, string title, SimEventTrigger? trigger = null) :
            this(item, trigger, title)
        {
        }

        public ToggleButtonModel(ReadOnlyBoolItem item, string title, SimEventTrigger trigger) :
            this(item, trigger, title)
        {
        }

        public ToggleButtonModel(IReadOnlyBoolItem item, SimEventTrigger? trigger, string title)
        {
            Title = title;
            this.item = item;
            simEvent = trigger;
            item.PropertyChanged += NotifyOnBoolValueChanged;
        }

        private void NotifyOnBoolValueChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals("BoolValue")??true)
                OnPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}