using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZXing;
using ZXing.Common;
using ZXing.Presentation;

namespace SimControls.Shell
{
    public partial class QRCodeDisplay : UserControl
    {
        public QRCodeDisplay()
        {
            InitializeComponent();
            this.SizeChanged += (_, __) => Regenerate();
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text",
            typeof(string), typeof(QRCodeDisplay), new FrameworkPropertyMetadata("", OnTextChange));

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        private static void OnTextChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QRCodeDisplay qrc) qrc.Regenerate();
        }
        
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.Register(
            "Geometry", typeof(Geometry), typeof(QRCodeDisplay), new PropertyMetadata(null));

        public Geometry Geometry
        {
            get => (Geometry) GetValue(GeometryProperty);
            set => SetValue(GeometryProperty, value);
        }

        private void Regenerate()
        {
            var writer = new BarcodeWriterGeometry()
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions(){Height = (int)ActualHeight, Width = (int)ActualWidth}
            };
            Geometry = writer.Write(Text);
        }
    }
}