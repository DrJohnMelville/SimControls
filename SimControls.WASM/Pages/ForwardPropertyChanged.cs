using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SimControls.WASM.Pages
{
    public class ForwardPropertyChanged: INotifyPropertyChanged
    {
        protected void RegisterPropertyChangeForwarding(params INotifyPropertyChanged[] elts)
        {
            foreach (var item in elts)
            {
                item.PropertyChanged += RelayPropertyChange;
            }
        }

        private void RelayPropertyChange(object? sender, PropertyChangedEventArgs e) => 
            PropertyChanged?.Invoke(sender, e);

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}