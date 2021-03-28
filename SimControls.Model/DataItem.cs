using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Melville.INPC;

namespace SimControls.Model
{
    public abstract  class DataItem: IExternalNotifyPropertyChanged
    {
        public ushort UniqueIndex { get; init; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public abstract string DebugValue { get; set; }
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public interface IReadOnlyDataItem<T> : INotifyPropertyChanged
    { 
        T Value { get; }
    }
    
    public class ReadOnlyDataItem<T> : DataItem, IReadOnlyDataItem<T>
    {
        protected T _value = default!;
        public T Value => _value;

        public override string DebugValue
        {
            get => _value?.ToString() ?? "<Null>";
            set { TryUpdateFromSimulator((T) Convert.ChangeType(value, typeof(T))); }
        }
        public bool TryUpdateFromSimulator(T newValue)
        {
            if (Value?.Equals(newValue) ?? false) return false;
            _value = newValue;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DebugValue));
            return true;
        }
    }
    public interface IDataItem<T> : IReadOnlyDataItem<T>
    { 
        new T Value { get; set; }
    }

    public  class DataItem<T> : ReadOnlyDataItem<T>, IDataItem<T>
    {
        public new T Value
        {
            get => _value;
            set
            {
                TryUpdateFromSimulator(value);
                OnUpdateSimulator();
            }
        }
        
        public event EventHandler<EventArgs>? WriteValueToSimulator;
        private void OnUpdateSimulator() => WriteValueToSimulator?.Invoke(this, EventArgs.Empty);
    }
}