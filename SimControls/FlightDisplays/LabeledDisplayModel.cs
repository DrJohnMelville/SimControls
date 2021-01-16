using System.Windows.Controls;

namespace SimControls.FlightDisplays
{
    public class LabeledDisplayModel: IDisplayModel 
    {
        public IDisplayModel Item { get; }
        public string Title { get; }
        public Dock Position { get; }

        public LabeledDisplayModel(IDisplayModel item, string title, Dock position)
        {
            Item = item;
            Title = title;
            Position = position;
        }
    }
}