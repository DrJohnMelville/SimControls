using System.Windows.Controls;
using SimControls.Model.CompositeElements;

namespace SimControls.FlightDisplays
{
    public class ConstrainedSliderModel : SliderModel
    {
        public ConstrainedSliderModel(BoundedDoubleItem item, Orientation orientation = Orientation.Vertical, bool isReversed = false) : base(item, orientation, isReversed)
        {
        }
    }
}