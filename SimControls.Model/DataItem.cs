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
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public interface IReadOnlyDataItem<T> : INotifyPropertyChanged
    { 
        T Value { get; }
    }
    
    public class ReadOnlyDataItem<T> : DataItem, IReadOnlyDataItem<T>
    {
        protected T value = default!;
        public T Value => value;
        public bool TryUpdateFromSimulator(T newValue)
        {
            if (Value?.Equals(newValue) ?? false) return false;
            value = newValue;
            OnPropertyChanged(nameof(Value));
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
}