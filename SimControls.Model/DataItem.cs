using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Melville.INPC;

namespace SimControls.Model
{
    public abstract  class DataItem: IExternalNotifyPropertyChanged
    {
        public ushort UniqueIndex { get; init; }
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public class ReadOnlyDataItem<T> : DataItem
    {
        protected T value = default!;
        public virtual T Value => value;
        public bool TryUpdateFromSimulator(T newValue)
        {
            if (Value?.Equals(newValue) ?? false) return false;
            value = newValue;
            OnPropertyChanged(nameof(Value));
            return true;
        }
    }

    public  class DataItem<T> : ReadOnlyDataItem<T>
    {
        public new T Value
        {
            get => value;
            set
            {
                TryUpdateFromSimulator(value);
                OnUpdateSimulator();
            }
        }
        
        public event EventHandler<EventArgs>? WriteValueToSimulator;
        private void OnUpdateSimulator() => WriteValueToSimulator?.Invoke(this, EventArgs.Empty);
    }

    public interface IReadOnlyBoolItem : INotifyPropertyChanged
    {
        public ushort UniqueIndex { get; }
        public bool BoolValue { get; }
    }
    
    public class ReadOnlyBoolItem : DataItem<int>, IReadOnlyBoolItem
    {
        public bool BoolValue => Value != 0;

        public ReadOnlyBoolItem()
        {
            this.DelegatePropertyChangeFrom(this, nameof(Value), nameof(BoolValue));
        }
    }
    
    public class BoolItem: DataItem<int>, IReadOnlyBoolItem
    {
        public bool BoolValue
        {
            get => Value != 0;
            set => Value = value ? 1 : 0;
        }
        public BoolItem()
        {
            this.DelegatePropertyChangeFrom(this, nameof(Value), nameof(BoolValue));
        }
    }
}