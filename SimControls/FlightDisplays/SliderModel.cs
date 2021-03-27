using System;
using System.Windows.Controls;
using SimControls.Model;
using SimControls.Model.CompositeElements;

namespace SimControls.FlightDisplays
{
    public class SliderModel : IDisplayModel
    {
        public BoundedDoubleItem Item { get; }
        public Orientation Orientation { get; }
        public bool IsReversed { get; }

        public SliderModel(BoundedDoubleItem item, 
            Orientation orientation = Orientation.Vertical, 
            bool isReversed = false)
        {
            Item = item;
            Orientation = orientation;
            IsReversed = isReversed;
        }
    }
}