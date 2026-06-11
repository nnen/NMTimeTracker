using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Brushes = System.Windows.Media.Brushes;
using Cursors = System.Windows.Input.Cursors;
using System.Windows.Media;
using NMTimeTracker.Model;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace NMTimeTracker.View
{
    public class DayTimelineControl : FrameworkElement
    {
        public static readonly DependencyProperty IntervalsProperty =
            DependencyProperty.Register(
                nameof(Intervals),
                typeof(IEnumerable<Interval>),
                typeof(DayTimelineControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnIntervalsChanged));

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register(
                nameof(Date),
                typeof(DateTime),
                typeof(DayTimelineControl),
                new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SelectedIntervalProperty =
            DependencyProperty.Register(
                nameof(SelectedInterval),
                typeof(Interval),
                typeof(DayTimelineControl),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TimelineStartProperty =
            DependencyProperty.Register(
                nameof(TimelineStart),
                typeof(TimeOnly?),
                typeof(DayTimelineControl),
                new FrameworkPropertyMetadata((TimeOnly?)new TimeOnly(6, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty TimelineEndProperty =
            DependencyProperty.Register(
                nameof(TimelineEnd),
                typeof(TimeOnly?),
                typeof(DayTimelineControl),
                new FrameworkPropertyMetadata((TimeOnly?)new TimeOnly(20, 0), FrameworkPropertyMetadataOptions.AffectsRender));

        public IEnumerable<Interval>? Intervals
        {
            get => (IEnumerable<Interval>?)GetValue(IntervalsProperty);
            set => SetValue(IntervalsProperty, value);
        }

        public DateTime Date
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public Interval? SelectedInterval
        {
            get => (Interval?)GetValue(SelectedIntervalProperty);
            set => SetValue(SelectedIntervalProperty, value);
        }

        public TimeOnly? TimelineStart
        {
            get => (TimeOnly?)GetValue(TimelineStartProperty);
            set => SetValue(TimelineStartProperty, value);
        }

        public TimeOnly? TimelineEnd
        {
            get => (TimeOnly?)GetValue(TimelineEndProperty);
            set => SetValue(TimelineEndProperty, value);
        }

        // Populated during OnRender for hit testing
        private List<(Interval Interval, Rect Rect)> m_renderedRects = new();

        private static void OnIntervalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DayTimelineControl)d;

            if (e.OldValue is IEnumerable<Interval> oldIntervals)
                foreach (var interval in oldIntervals)
                    interval.PropertyChanged -= control.OnIntervalPropertyChanged;
            if (e.OldValue is INotifyCollectionChanged oldCol)
                oldCol.CollectionChanged -= control.OnCollectionChanged;

            if (e.NewValue is IEnumerable<Interval> newIntervals)
                foreach (var interval in newIntervals)
                    interval.PropertyChanged += control.OnIntervalPropertyChanged;
            if (e.NewValue is INotifyCollectionChanged newCol)
                newCol.CollectionChanged += control.OnCollectionChanged;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Interval i in e.OldItems)
                    i.PropertyChanged -= OnIntervalPropertyChanged;
            if (e.NewItems != null)
                foreach (Interval i in e.NewItems)
                    i.PropertyChanged += OnIntervalPropertyChanged;
            Dispatcher.InvokeAsync(InvalidateVisual);
        }

        private void OnIntervalPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Interval.End) || e.PropertyName == nameof(Interval.Start))
                Dispatcher.InvokeAsync(InvalidateVisual);
        }

        // Make the entire element area hit-testable (not just painted pixels)
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) =>
            new PointHitTestResult(this, hitTestParameters.HitPoint);

        protected override void OnRender(DrawingContext dc)
        {
            double width = ActualWidth;
            double height = ActualHeight;
            if (width <= 0 || height <= 0) return;

            dc.DrawRectangle(
                new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                new Pen(new SolidColorBrush(Color.FromRgb(160, 160, 160)), 1.0),
                new Rect(0.5, 0.5, width - 1, height - 1));

            var dayStart = Date.Date;
            var dayEnd = dayStart.AddDays(1);

            // Base range from TimelineStart/TimelineEnd; expanded outward to cover all intervals
            DateTime? rangeStart = TimelineStart.HasValue ? dayStart + TimelineStart.Value.ToTimeSpan() : null;
            DateTime? rangeEnd   = TimelineEnd.HasValue   ? dayStart + TimelineEnd.Value.ToTimeSpan()   : null;

            var intervals = Intervals?.ToList();
            m_renderedRects = new List<(Interval, Rect)>();

            if (intervals != null && intervals.Count > 0)
            {
                var iMin = intervals.Select(i => i.Start < dayStart ? dayStart : i.Start).Min();
                var iMax = intervals.Select(i => i.End   > dayEnd   ? dayEnd   : i.End  ).Max();
                rangeStart = rangeStart.HasValue ? (rangeStart.Value < iMin ? rangeStart.Value : iMin) : iMin;
                rangeEnd   = rangeEnd.HasValue   ? (rangeEnd.Value   > iMax ? rangeEnd.Value   : iMax) : iMax;
            }

            if (!rangeStart.HasValue || !rangeEnd.HasValue) return;

            double totalSeconds = (rangeEnd.Value - rangeStart.Value).TotalSeconds;
            if (totalSeconds <= 0) return;

            var selectedInterval = SelectedInterval;

            if (intervals != null)
            {
                foreach (var interval in intervals)
                {
                    var start = interval.Start < dayStart ? dayStart : interval.Start;
                    var end   = interval.End   > dayEnd   ? dayEnd   : interval.End;
                    if (end <= start) continue;

                    double x = (start - rangeStart.Value).TotalSeconds / totalSeconds * width;
                    double w = Math.Max(1.0, (end - start).TotalSeconds / totalSeconds * width);
                    var rect = new Rect(x, 1, w, height - 2);

                    bool isSelected = (interval == selectedInterval);
                    var brush = isSelected ? GetSelectedBrush(interval.StartReason) : GetBrush(interval.StartReason);
                    dc.DrawRectangle(brush, null, rect);
                    m_renderedRects.Add((interval, rect));
                }
            }

            // Draw hour and half-hour tick marks on top of the intervals
            var hourPen   = new Pen(new SolidColorBrush(Color.FromArgb( 90, 0, 0, 0)), 1.0);
            var halfHrPen = new Pen(new SolidColorBrush(Color.FromArgb( 50, 0, 0, 0)), 1.0);
            hourPen.Freeze();
            halfHrPen.Freeze();

            var tick = rangeStart.Value.Date.AddMinutes(Math.Ceiling(rangeStart.Value.TimeOfDay.TotalMinutes / 30.0) * 30);
            while (tick <= rangeEnd.Value)
            {
                double x = (tick - rangeStart.Value).TotalSeconds / totalSeconds * width;
                bool isHour = (tick.Minute == 0);
                dc.DrawLine(isHour ? hourPen : halfHrPen, new Point(x, 1), new Point(x, height - 1));
                tick = tick.AddMinutes(30);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var pos = e.GetPosition(this);
            foreach (var (interval, rect) in m_renderedRects)
            {
                if (rect.Contains(pos))
                {
                    SelectedInterval = interval;
                    e.Handled = true;
                    return;
                }
            }
            SelectedInterval = null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var pos = e.GetPosition(this);
            Cursor = m_renderedRects.Any(r => r.Rect.Contains(pos)) ? Cursors.Hand : Cursors.Arrow;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            Cursor = Cursors.Arrow;
        }

        private static readonly Dictionary<TimeTrackerEvents, Color> s_colors = new()
        {
            [TimeTrackerEvents.UserStart]        = Color.FromRgb( 76, 175,  80),
            [TimeTrackerEvents.AppStartup]       = Color.FromRgb( 33, 150, 243),
            [TimeTrackerEvents.SessionUnlock]    = Color.FromRgb(  0, 188, 212),
            [TimeTrackerEvents.ScreensaverStart] = Color.FromRgb(255, 193,   7),
        };
        private static readonly Color s_defaultColor = Color.FromRgb(158, 158, 158);

        private static readonly Dictionary<TimeTrackerEvents, Brush> s_brushes =
            s_colors.ToDictionary(kv => kv.Key, kv => (Brush)new SolidColorBrush(kv.Value));
        private static readonly Brush s_defaultBrush = new SolidColorBrush(s_defaultColor);

        private static readonly Dictionary<TimeTrackerEvents, Brush> s_selectedBrushes =
            s_colors.ToDictionary(kv => kv.Key, kv => CreateDiagonalStripeBrush(kv.Value));
        private static readonly Brush s_defaultSelectedBrush = CreateDiagonalStripeBrush(s_defaultColor);

        private static Brush GetBrush(TimeTrackerEvents reason) =>
            s_brushes.TryGetValue(reason, out var brush) ? brush : s_defaultBrush;

        private static Brush GetSelectedBrush(TimeTrackerEvents reason) =>
            s_selectedBrushes.TryGetValue(reason, out var brush) ? brush : s_defaultSelectedBrush;

        // Builds a tiled 45° diagonal stripe brush: base color alternating with semi-transparent white.
        // The tile is 8×8 px; stripes are 4 px wide at 45°, formed by two polygons whose union
        // covers every pixel where (x+y) mod 8 < 4.
        private static Brush CreateDiagonalStripeBrush(Color baseColor)
        {
            const double t = 12;  // tile size
            const double h = 6;  // half tile (stripe width)

            var group = new DrawingGroup();
            group.Children.Add(new GeometryDrawing(
                new SolidColorBrush(baseColor), null,
                new RectangleGeometry(new Rect(0, 0, t, t))));

            // Top-left triangle: (0,0)→(h,0)→(0,h)
            var fig1 = new PathFigure { StartPoint = new Point(0, 0), IsClosed = true };
            fig1.Segments.Add(new LineSegment(new Point(h, 0), false));
            fig1.Segments.Add(new LineSegment(new Point(0, h), false));

            // Main stripe band across the tile: (0,t)→(t,0)→(t,h)→(h,t)
            var fig2 = new PathFigure { StartPoint = new Point(0, t), IsClosed = true };
            fig2.Segments.Add(new LineSegment(new Point(t, 0), false));
            fig2.Segments.Add(new LineSegment(new Point(t, h), false));
            fig2.Segments.Add(new LineSegment(new Point(h, t), false));

            group.Children.Add(new GeometryDrawing(
                new SolidColorBrush(Color.FromArgb(160, 255, 255, 255)), null,
                new PathGeometry(new[] { fig1, fig2 })));

            var brush = new DrawingBrush
            {
                Drawing = group,
                TileMode = TileMode.Tile,
                Viewport = new Rect(0, 0, t, t),
                ViewportUnits = BrushMappingMode.Absolute,
            };
            brush.Freeze();
            return brush;
        }
    }
}
