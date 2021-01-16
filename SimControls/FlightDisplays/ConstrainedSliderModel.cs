using System.Windows.Controls;
using SimControls.Model;

namespace SimControls.FlightDisplays
{
    public class ConstrainedSliderModel : IDisplayModel
    {
        public DataItem<double> Position { get; }
        public ReadOnlyDataItem<double> TotalPositions { get; }
        public Orientation Orientation { get; }
        public bool IsReversed { get; }

        public ConstrainedSliderModel(DataItem<double> position, ReadOnlyDataItem<double> totalPositions,
            Orientation orientation, bool isReversed)
        {
            Position = position;
            TotalPositions = totalPositions;
            Orientation = orientation;
            IsReversed = isReversed;
        }
    }
}