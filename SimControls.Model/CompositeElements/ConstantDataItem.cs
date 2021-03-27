using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimControls.Model.CompositeElements
{
    public static class ConstantDataItem
    {
        public static IReadOnlyDataItem<T> FromValue<T>(T value) => new ConstantDataItem<T>(value);
    }
    
    public class ConstantDataItem<T>: IReadOnlyDataItem<T>
    {
        // Since the value is a constant, ignore attempts to register for change notification
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add {  }
            remove {  }
        }

        public T Value { get; }

        public ConstantDataItem(T value)
        {
            Value = value;
        }
    }

}