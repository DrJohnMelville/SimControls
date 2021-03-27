using System;
using System.Windows;
using Melville.MVVM.BusinessObjects;
using SimControls.Model;
using SimControls.Model.CompositeElements;

namespace SimControls.FlightDisplays
{
    public class DoubleUpDownDisplayModel : NotifyBase, IDisplayModel
    {
        public BoundedDoubleItem Item { get; }

        public DoubleUpDownDisplayModel(BoundedDoubleItem item)
        {
            Item = item;
        }

        public bool SmallIncrementVisible => Item is SingleStepDoubleItem;
        public bool BigImprementsVisible => Item is TwoStepDoubleItem;
    }
}