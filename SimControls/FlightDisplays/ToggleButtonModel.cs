using SimControls.Model;

namespace SimControls.FlightDisplays
{
    public class ToggleButtonModel : IDisplayModel
    {
        public string Title { get; }
        public IBoolItem Item { get; }

        public ToggleButtonModel(string title, IBoolItem item)
        {
            Title = title;
            Item = item;
        }
    }
}