using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Wpf.Bindings;
using SimControls.Model.AirportDatabase;

namespace SimControls.FlightDisplays
{

    public class NullLongListSource : IAirportRepository
    {
        public IAsyncEnumerable<Airport> SearchAirports(string searchString) => 
            Array.Empty<Airport>().ToAsyncEnumerable();
    }

    public partial class SelectFromLongList : UserControl
    {
        private readonly RepeatedEventFilter repeatFilter = new();
        public SelectFromLongList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(IAirportRepository), typeof(SelectFromLongList),
            new FrameworkPropertyMetadata(new NullLongListSource()));

        public IAirportRepository Source
        {
            get => (IAirportRepository) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(object), typeof(SelectFromLongList),
            new FrameworkPropertyMetadata(null));
        
        public object? Value
        {
            get => (object?) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly IValueConverter ShowIfNotEmpty = LambdaConverter.Create(
            (IList<object> obj) => obj.Count > 0);

        private void NewQueryString(object sender, TextChangedEventArgs e)
        {
            if (QueryBox.Text == Value?.ToString()) return;
            Value = null;
            RequeryAsync().FireAndForget();
        }

        private async Task RequeryAsync()
        {
            if (await repeatFilter.DoesSucceedingEventHappenAsync(TimeSpan.FromSeconds(0.5))) return;
            SetCandidates(await Source.SearchAirports(QueryBox.Text).OfType<object>().ToListAsync());
        }

        private void SetCandidates(IList<object> candidates)
        {
            Popup.IsOpen = candidates.Count > 0;
            CandidateBox.ItemsSource = candidates;
        }

        private void NewSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            var newValue = e.AddedItems.OfType<object>().FirstOrDefault();
            Value = newValue;
            if (newValue != null)
            {
                QueryBox.Text = newValue.ToString() ?? "";
            }
        }

        private void UserLeftControl(object sender, RoutedEventArgs e) => CloseBoxAndSeleect();

        private void CloseBoxAndSeleect()
        {
            TrySetToFirstCandidate();
            Popup.IsOpen = false;
            QueryBox.Text = Value?.ToString() ?? QueryBox.Text;
        }

        private void TrySetToFirstCandidate()
        {
            Value ??= (CandidateBox?.ItemsSource as IList<object>)?.FirstOrDefault();
        }

        private void CheckForEnterControl(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) CloseBoxAndSeleect();
        }
    }

    public class RepeatedEventFilter
    {
        private volatile int eventCount = 0;

        public async Task<bool> DoesSucceedingEventHappenAsync(TimeSpan span)
        {
            var myTicketNumber = Interlocked.Increment(ref eventCount);
            await Task.Delay(span);
            return myTicketNumber != eventCount;
        } 
    }
    
}
