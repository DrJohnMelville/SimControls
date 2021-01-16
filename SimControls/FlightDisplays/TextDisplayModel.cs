using SimControls.Model;

namespace SimControls.FlightDisplays
{
    public class TextDisplayModel : IDisplayModel
    {
        public DataItem Item { get; }

        public TextDisplayModel(DataItem item)
        {
            Item = item;
        }
    }
}