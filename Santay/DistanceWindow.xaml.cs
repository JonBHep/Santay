using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Santay;

public partial class DistanceWindow : Window
{
    private readonly VeloHistory _history;
        private readonly double _xInterval = 3;
        // private DateTime _minDate;
        // private DateTime _maxDate;
        private DateOnly _lowDate;
        private DateOnly _topDate;
        
        private readonly Brush _brush30 = Brushes.Blue;
        private readonly Brush _brush90 = Brushes.Magenta;
        private readonly Brush _brushYr = Brushes.Red;

        public DistanceWindow(VeloHistory story)
        {
            InitializeComponent();
            _history = story;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double scrX = SystemParameters.PrimaryScreenWidth;
            double scrY = SystemParameters.PrimaryScreenHeight;
            double winX = scrX * .95;
            double winY = scrY * .9;
            double Xm = (scrX - winX) / 2;
            double Ym = (scrY - winY) / 4;
            Width = winX;
            Height = winY;
            Left = Xm;
            Top = Ym;
            YearBloc.Foreground = _brushYr;
            Bloc30.Foreground = _brush30;
            Bloc90.Foreground = _brush90;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            DisplayDateFramework();
            DrawDistanceChart();
            DrawFrequencyChart();
        }

        private void DisplayDateFramework()
        {
            double canvasHeight = ChartCanvas.ActualHeight;
            List<Balade> riderBallades = _history.TripList;
            ChartCanvas.Children.Clear();
            //_minDate = riderBallades[0].RideDate.Date.AddYears(1);
            _lowDate = riderBallades[0].TripDate.AddYears(1);
            //_maxDate = _history.NotionalToday;
            _topDate = _history.NotionalNow;
            var ff = _topDate.DayNumber - _lowDate.DayNumber;
            //TimeSpan dateInterval = _maxDate - _minDate;
            double dayspan =ff + 1;
            ChartCanvas.Width = _xInterval * dayspan;
            DateOnly p = _lowDate;
            do
            {
                if (p.Day == 1)
                {
                    double xpos = XPosition(p);
                    Line monthstarter = new Line() { X1 = xpos, X2 = xpos, Y1 = 0, Y2 = canvasHeight, Stroke = Brushes.Black, StrokeThickness = 0.8, StrokeDashArray = new DoubleCollection { 6, 3 } };
                    ChartCanvas.Children.Add(monthstarter);
                    TextBlock monthstarterblock = new TextBlock() { Text = p.ToString("MMM yyyy"), Foreground = Brushes.Black };
                    Canvas.SetLeft(monthstarterblock, xpos + 4);
                    Canvas.SetTop(monthstarterblock, 8);
                    ChartCanvas.Children.Add(monthstarterblock);
                }
                p = p.AddDays(1);
            } while (p < _topDate);

            ChartScrollViewer.ScrollToRightEnd();
        }

        // private double XPosition(DateTime d)
        // {
        //     TimeSpan t = d - _minDate;
        //     return t.TotalDays * _xInterval;
        // }
        private double XPosition(DateOnly d)
        {
            int tt = d.DayNumber - _lowDate.DayNumber;
            return tt * _xInterval;
        }

        // private double RollingPeriodMeanDailyDistance(DateTime periodEnd, int days)
        // {
        //     DateTime periodStart = periodEnd.AddDays(-(days - 1));
        //     double dist = _history.PeriodDistance(periodStart, periodEnd);
        //     return dist / days;
        // }
        private double RollingPeriodMeanDailyDistance(DateOnly periodEnd, int days)
        {
            DateOnly periodStart = periodEnd.AddDays(-(days - 1));
            double dist = _history.PeriodDistance(periodStart, periodEnd);
            return dist / days;
        }
        // private double RollingPeriodMeanRidesPerWeek(DateTime periodEnd, int days)
        // {
        //     DateTime periodStart = periodEnd.AddDays(-(days - 1));
        //     return _history.PeriodTripCount(periodStart, periodEnd) / ((double)days / 7);
        // }
        private double RollingPeriodMeanRidesPerWeek(DateOnly periodEnd, int days)
        {
            DateOnly periodStart = periodEnd.AddDays(-(days - 1));
            return _history.PeriodTripCount(periodStart, periodEnd) / ((double)days / 7);
        }
        private void DrawDistanceChart()
        {
            var seqYr = new List<Tuple<DateOnly, double>>();
            var seq90 = new List<Tuple<DateOnly, double>>();
            var seq30 = new List<Tuple<DateOnly, double>>();
            DateOnly quand = _lowDate;
            while (quand.DayNumber <= _topDate.DayNumber)
            {
                var vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanDailyDistance(quand, 365));
                seqYr.Add(vlu);
                vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanDailyDistance(quand, 90));
                seq90.Add(vlu);
                vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanDailyDistance(quand, 30));
                seq30.Add(vlu);
                quand = quand.AddDays(1);
            }
            double maxheight = 0;
            foreach (var R in seqYr)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }
            foreach (var R in seq90)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }
            foreach (var R in seq30)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }

            double actuality = ChartCanvas.ActualHeight;
            double chartheight = actuality / 2;
            double lowermargin = 20;
            double chartceiling = 0;

            double vratio = (chartheight - 40) / maxheight;
            double yy = chartceiling + chartheight - lowermargin;
            Line baseline = new Line() { X1 = 0, X2 = XPosition(DateOnly.FromDateTime(DateTime.Today)), Y1 = yy, Y2 = yy, Stroke = Brushes.DimGray, StrokeThickness = 2 };
            ChartCanvas.Children.Add(baseline);

            for (int w = 1; w < 10; w++)
            {
                yy = chartceiling + chartheight - (lowermargin + vratio * w);
                if (yy > chartceiling)
                {
                    baseline = new Line() { X1 = 0, X2 = XPosition(DateOnly.FromDateTime(DateTime.Today)), Y1 = yy, Y2 = yy, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5, Opacity = 0.5 };
                    ChartCanvas.Children.Add(baseline);
                }
            }

            PlotProgress(seqYr, chartceiling + chartheight, lowermargin, vratio, _brushYr);
            PlotProgress(seq90, chartceiling + chartheight, lowermargin, vratio, _brush90);
            PlotProgress(seq30, chartceiling + chartheight, lowermargin, vratio, _brush30);
        }

        private void DrawFrequencyChart()
        {
            var seqYr = new List<Tuple<DateOnly, double>>();
            var seq90 = new List<Tuple<DateOnly, double>>();
            var seq30 = new List<Tuple<DateOnly, double>>();
            DateOnly quand =_lowDate;
            while (quand.DayNumber <= _topDate.DayNumber)
            {
                var vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanRidesPerWeek(quand, 365));
                seqYr.Add(vlu);
                vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanRidesPerWeek(quand, 90));
                seq90.Add(vlu);
                vlu = new Tuple<DateOnly, double>(quand, RollingPeriodMeanRidesPerWeek(quand, 30));
                seq30.Add(vlu);
                quand = quand.AddDays(1);
            }
            double maxheight = 0;
            foreach (var R in seqYr)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }
            foreach (var R in seq90)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }
            foreach (var R in seq30)
            {
                maxheight = Math.Max(R.Item2, maxheight);
            }

            double actuality = ChartCanvas.ActualHeight;
            double chartheight = actuality / 2;
            double lowermargin = 20;
            double chartceiling = chartheight;

            double vratio = (chartheight - 40) / maxheight;
            double yy = chartceiling + chartheight - lowermargin;
            Line baseline = new Line() { X1 = 0, X2 = XPosition(DateOnly.FromDateTime(DateTime.Today)), Y1 = yy, Y2 = yy, Stroke = Brushes.DimGray, StrokeThickness = 2 };
            ChartCanvas.Children.Add(baseline);

            for (double w = 0; w <= 7; w += 1)
            {
                yy = chartceiling + chartheight - (lowermargin + vratio * w);
                if (yy > chartceiling)
                {
                    baseline = new Line() { X1 = 0, X2 = XPosition(DateOnly.FromDateTime(DateTime.Today)), Y1 = yy, Y2 = yy, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5, Opacity = 0.5 };
                    ChartCanvas.Children.Add(baseline);
                }
            }

            PlotProgress(seqYr, chartceiling + chartheight, lowermargin, vratio, _brushYr);
            PlotProgress(seq90, chartceiling + chartheight, lowermargin, vratio, _brush90);
            PlotProgress(seq30, chartceiling + chartheight, lowermargin, vratio, _brush30);
        }

        private void PlotProgress(List<Tuple<DateOnly, double>> sequence, double chartheight, double bottommargin, double vertratio, Brush pinceau)
        {
            var  S = sequence[0];
            double xp = XPosition(S.Item1);
            double yp = chartheight - (bottommargin + vertratio * S.Item2);
            sequence.RemoveAt(0);
            foreach (var R in sequence)
            {
                double xn = XPosition(R.Item1);
                double yn = chartheight - (bottommargin + vertratio * R.Item2);
                Line stick = new Line() { X1 = xp, X2 = xn, Y1 = yp, Y2 = yn, Stroke = pinceau, StrokeThickness = 1 };
                ChartCanvas.Children.Add(stick);
                xp = xn; yp = yn;
            }
            Line rebound = new Line() { X1 = 0, X2 = xp, Y1 = yp, Y2 = yp, Stroke = pinceau, StrokeThickness = 1, Opacity = 0.4, StrokeDashArray = { 16, 16 } };
            ChartCanvas.Children.Add(rebound);
        }
}