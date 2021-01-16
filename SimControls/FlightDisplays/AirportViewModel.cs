using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Melville.MVVM.BusinessObjects;
using Melville.MVVM.Functional;
using SimControls.Model.AirportDatabase;

namespace SimControls.FlightDisplays
{
    public class AirportViewModel : NotifyBase
    {
        public Airport Airport { get; }
        private Runway? selectedRunway = null;
        public Runway? SelectedRunway
        {
            get => selectedRunway;
            set => AssignAndNotify(ref selectedRunway, value);
        }

        public IList<Runway> DisplayRunways { get; } 

        public AirportViewModel(Airport airport)
        {
            Airport = airport;
            DisplayRunways = airport.Runways.SelectMany(i => new[] {i, i.ReverseRunway}).ToList();
        }
        
    }

    public struct Dimension
    {
        public double Min { get; }
        public double Max { get; }
        public double Mean => (Max + Min) / 2.0;
        public double Length => Max - Min;
        public Dimension(IEnumerable<double> values)
        {
            (Min, Max) = values.MinMax();
        }
    }
    
    public class AirportDiagram : FrameworkElement
    {
        public static DependencyProperty RunwaysProperty = DependencyProperty.Register("Runways",
            typeof(IList<Runway>), typeof(AirportDiagram), new PropertyMetadata(null));

        public IList<Runway>? Runways
        {
            get => (IList<Runway>)GetValue(RunwaysProperty);
            set => SetValue(RunwaysProperty, value);
        }

        public static DependencyProperty SelectedRunwayProperty = DependencyProperty.Register(
            "SelectedRunway", typeof(Runway), typeof(AirportDiagram), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        public Runway? SelectedRunway
        {
            get => (Runway)GetValue(SelectedRunwayProperty);
            set => SetValue(SelectedRunwayProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (!(Runways is {} runways)) return;
            var blackPen = new Pen(Brushes.Black, 2);
            var redPen = new Pen(Brushes.Red, 2);
            var xform = ComputeTransform(runways);
            foreach (var runway in Runways.Where(I=>I.RunwayId > 0))
            {
                dc.DrawLine(IsSelectedRunway(runway)? redPen:blackPen, 
                    ComputePoint(xform, runway.LonX, runway.LatY),
                    ComputePoint(xform, runway.ReverseLonX, runway.ReverseLatY));
            }
        }

        private bool IsSelectedRunway(Runway runway)
        {
            return runway.Name == SelectedRunway?.Name || runway.ReverseName == SelectedRunway?.Name;
        }

        private Point ComputePoint(Transform xform, in double x, in double y) => 
            xform.Transform(new Point(x, y));

        private Transform ComputeTransform(IList<Runway> runways)
        {
            var ret = new TransformGroup();
            var xDimension = new Dimension(runways.Select(i => i.LonX));
            var yDimension = new Dimension(runways.Select(i => i.LatY));
            ret.Children.Add(new TranslateTransform(-xDimension.Mean, - yDimension.Mean));
            var zoomX = ActualWidth / xDimension.Length;
            var zoomY = ActualHeight / yDimension.Length;
            var zoom = Math.Min(zoomX, zoomY);
            ret.Children.Add(new ScaleTransform(zoom, -zoom));
            ret.Children.Add(new TranslateTransform(ActualWidth/2.0, ActualHeight/2.0));
            return ret;
        }
    }
}