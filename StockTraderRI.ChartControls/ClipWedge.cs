

using System.Windows;
using System.Windows.Media;
using System;
using System.Windows.Controls;

namespace StockTraderRI.ChartControls
{
    public class ClipWedge : ContentControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ClipWedge()
        {
            _r = new Random();
        }

        public static void OnWedgeShapeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ClipWedge c = sender as ClipWedge;
            if(c!=null)
                c.InvalidateArrange();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Clip = GetClipGeometry(arrangeBounds);
            return base.ArrangeOverride(arrangeBounds);
        }

        public StreamGeometry GetClipGeometry(Size arrangeBounds)
        {
            StreamGeometry clip = new StreamGeometry();
            StreamGeometryContext clipGc = clip.Open();
            clipGc.BeginFigure(BeginFigurePoint, true, true);
            clipGc.LineTo(LineToPoint, false, true);
            Vector v = LineToPoint - BeginFigurePoint;
            RotateTransform rt = new RotateTransform(WedgeAngle, BeginFigurePoint.X, BeginFigurePoint.Y);
            bool isLargeArc = WedgeAngle >180.0;
            clipGc.ArcTo(rt.Transform(LineToPoint), new Size(v.Length, v.Length), WedgeAngle, isLargeArc, SweepDirection.Clockwise, false, true);
            clipGc.Close();
            return clip;
        }

        public double WedgeAngle
        {
            get => (double)GetValue(WedgeAngleProperty);
            set => SetValue(WedgeAngleProperty, value);
        }

        // Using a DependencyProperty as the backing store for WedgeAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WedgeAngleProperty =
            DependencyProperty.Register("WedgeAngle", typeof(double), typeof(ClipWedge), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnWedgeShapeChanged)));


        public Point BeginFigurePoint
        {
            get => (Point)GetValue(BeginFigurePointProperty);
            set => SetValue(BeginFigurePointProperty, value);
        }

        // Using a DependencyProperty as the backing store for BeginFigurePoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginFigurePointProperty =
            DependencyProperty.Register("BeginFigurePoint", typeof(Point), typeof(ClipWedge), new UIPropertyMetadata(new Point(), new PropertyChangedCallback(OnWedgeShapeChanged)));


        public Point LineToPoint
        {
            get => (Point)GetValue(LineToPointProperty);
            set => SetValue(LineToPointProperty, value);
        }

        // Using a DependencyProperty as the backing store for LineTo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineToPointProperty =
            DependencyProperty.Register("LineToPoint", typeof(Point), typeof(ClipWedge), new UIPropertyMetadata(new Point(), new PropertyChangedCallback(OnWedgeShapeChanged)));

        private static Random _r;
    }
}