using System;
using System.Windows.Controls;
using SimControls.Model;

namespace SimControls.FlightDisplays
{
    public class SliderModel : DisplayModel<double>
    {
        public double Minimum { get; }
        public double Maximum { get; }
        public Orientation Orientation { get; }
        public bool IsReversed { get; }

        public SliderModel(DataItem<double> item, double minimum, double maximum, Orientation orientation) : base(item)
        {
            Minimum = Math.Min(minimum, maximum);
            Maximum = Math.Max(minimum, maximum);
            IsReversed = minimum > maximum;
            Orientation = orientation;
        }
    }
}