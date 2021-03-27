using System.ComponentModel;
using Melville.INPC;

namespace SimControls.Model
{
    public interface IReadOnlyBoolItem : INotifyPropertyChanged
    {
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

    public interface IBoolItem: IReadOnlyBoolItem
    {
        new bool BoolValue { get; set; }
    }
    
    public class BoolItem: DataItem<int>, IBoolItem
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