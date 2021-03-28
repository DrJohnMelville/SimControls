using System;
using System.ComponentModel;
using System.Linq;
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

        private static readonly char[] trueValues = new[]
            {'t', 'y', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public override string DebugValue
        {
            get => BoolValue.ToString();
            set => base.DebugValue = value.Length > 0 && trueValues.Contains(Char.ToLower(value[0]))?
                "1":"0";
        }

        public ReadOnlyBoolItem()
        {
            this.DelegatePropertyChangeFrom(this, nameof(Value), nameof(BoolValue));
        }
    }

    public interface IBoolItem: IReadOnlyBoolItem
    {
        new bool BoolValue { get; set; }
        
    }

    public static class BoolItemOperations
    {
        public static void Toggle(this IBoolItem item) =>
            item.BoolValue = !item.BoolValue;
    }

    public class BoolItem: ReadOnlyBoolItem, IBoolItem
    {
        public new bool BoolValue
        {
            get => Value != 0;
            set => Value = value ? 1 : 0;
        }
        public BoolItem()
        {
        }
    }
}