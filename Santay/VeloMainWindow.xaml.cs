using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Santay;

public partial class VeloMainWindow
{
    public VeloMainWindow()
        {
            InitializeComponent();
            _history = new VeloHistory("Data.velo");
        }

        private readonly VeloHistory _history;
        private const string DistFormat = "0.0 km";
        private readonly List<TextBlock> _signs = new List<TextBlock>();
        private int _plot;
        private double _canvasWidth;
        private readonly double _xInterval = 4;
        private DateOnly _minDate;
        private DateOnly _maxDate;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var scrX = SystemParameters.PrimaryScreenWidth;
            var scrY = SystemParameters.PrimaryScreenHeight;
            var winX = scrX * .95;
            var winY = scrY * .9;
            var xm = (scrX - winX) / 2;
            var ym = (scrY - winY) / 4;
            Width = winX;
            Height = winY;
            Left = xm;
            Top = ym;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            RefreshDisplay();
            DisplayVersionInfo();
        }

        private void RefreshDisplay()
        {
            DisplayStatistics();
            DisplayDateFramework();
            DisplayRidesGraph();
        }

        private void DisplayVersionInfo()
        {
            TodayTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(42, 133, 83));
            TodayTextBlock.Text = "Today is " + DateTime.Today.ToString("ddd dd MMM yyyy");
        }

        private void DisplayStatistics()
        {
            int tt = _history.TripCountCycling;
            int td = _history.TotalDaysCycling;
            CountTbkVelo.Text = $"{tt} rides in {td} days";
            double dd = td / (double)tt;
            CountTbkVelo.ToolTip = $"A ride every {dd:0.0} days";
            if (_history.TripCountCycling > 0)
            {
                MaximumTbkVelo.Text = $"Longest ride {_history.MaximumTripKmVelo.ToString(DistFormat)}";
                MeanTbkVelo.Text = $"Mean ride {_history.AverageTripKmVelo.ToString(DistFormat)}";
                KmPerDayTbkVelo.Text = $"Mean {_history.AverageDailyKmVelo.ToString(DistFormat)} / day";

                string mean = _history.Average4WeeklyKmVelo.ToString(DistFormat);
                string roll = (_history.RollingMonth.TripCountVelo == 0) ? "no rides" : (_history.RollingMonth.TripCountVelo == 1) ? $"One ride of {_history.RollingMonth.RiddenKilometres.ToString(DistFormat)}." : $"{_history.RollingMonth.RiddenKilometres.ToString(DistFormat)} ({_history.RollingMonth.TripCountVelo} rides)";
                KmPerMonthTbkVelo.Text = $"Month: mean {mean}, latest {roll}";
                double mix = _history.RollingMonth.RiddenKilometres / _history.Average4WeeklyKmVelo; // compares actual with average
                                                                                                     // convert to a double between 0 and 1 with 0.5 = current matches average
                mix /= 2;
                mix = Math.Min(1, mix); // in case the actual was more than twice the average
                PerMonthFlagVelo.Fill = new SolidColorBrush(VeloHistory.ColourMix(Colors.LightCoral, Colors.DarkSeaGreen, mix));
                PerMonthFlagVelo.ToolTip = mix.ToString(CultureInfo.CurrentCulture);
            }
        }

        private double XPosition(DateOnly d)
        {
            int tt = d.DayNumber-_minDate.DayNumber;
            return tt * _xInterval;
        }
        private void DisplayRidesGraph()
        {
            double canvasHeight = ChartCanvas.ActualHeight;

            List<Balade> riderBallades = _history.TripList;

            double maxDist = double.MinValue;

            foreach (Balade b in riderBallades)
            {
                maxDist = Math.Max(maxDist, b.RideKm);
            }

            // add rides
            foreach (Balade b in riderBallades)
            {
                double xpos = XPosition(b.TripDate);
                double ypos = canvasHeight - (b.RideKm * canvasHeight / maxDist);
                Brush scb = VeloHistory.BrushEasy;
                if (b.Difficulty == 2) { scb = VeloHistory.BrushIntermediate; }
                if (b.Difficulty == 3) { scb = VeloHistory.BrushHard; }
                Line l = new Line() { X1 = xpos, X2 = xpos, Y1 = canvasHeight, Y2 = ypos, StrokeThickness = _xInterval - 1, Stroke = scb };
                ChartCanvas.Children.Add(l);
            }

            // draw a horizontal line at each 10K
            for (double km = 0; km < 1200; km += 10)
            {
                double ypos = canvasHeight - (km * canvasHeight / maxDist);
                if ((ypos > 0) && (ypos < canvasHeight))
                {
                    Line l = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = ypos, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.Ivory, Opacity = 0.3 };
                    ChartCanvas.Children.Add(l);

                    TextBlock tl = new TextBlock() { Text = $" {km} km ", Foreground = Brushes.Silver, Background = ChartCanvas.Background };
                    _signs.Add(tl);
                    Canvas.SetTop(tl, ypos - 8);
                    Canvas.SetLeft(tl, 100);
                    ChartCanvas.Children.Add(tl);
                }
            }

            //// draw a horizontal line at the average distance value for cycling
            double yavg;
            yavg = canvasHeight - (_history.AverageTripKmVelo * canvasHeight / maxDist);
            Line m = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = yavg, Y2 = yavg, StrokeThickness = 1.5, Stroke = Brushes.LightCoral, Opacity = 0.5 };
            ChartCanvas.Children.Add(m);

            TextBlock tml = new TextBlock() { Text = " Mean ride distance ", Foreground = Brushes.LightCoral, Background = ChartCanvas.Background, Margin = new Thickness(100, 0, 0, 0) };
            Canvas.SetTop(tml, yavg - 8);
            Canvas.SetLeft(tml, 100);
            ChartCanvas.Children.Add(tml);
            _signs.Add(tml);
        
            // tml = new TextBlock() { Text = " Mean walk distance ", FontWeight = FontWeights.Light, Foreground = Brushes.LightCoral, Background = ChartCanvas.Background, Margin = new Thickness(100, 0, 0, 0) };
            // Canvas.SetTop(tml, yavg - 8);
            // Canvas.SetLeft(tml, 100);
            // ChartCanvas.Children.Add(tml);
            // _signs.Add(tml);

            ChartScrollViewer.ScrollToRightEnd();
        }

        private void DisplayCumulativeGraph()
        {
            var cumulus = new List<Tuple<double, DateOnly>>();
            double cumv = 0;

            double canvasHeight = ChartCanvas.ActualHeight;

            List<Balade> riderBallades = _history.TripList;

            foreach (Balade b in riderBallades)
            {
                cumv += b.RideKm;
                var cv = new Tuple<double, DateOnly>(cumv, b.TripDate);
                cumulus.Add(cv);
            }

            // add line for cumulative distance
            Point last = new Point(-1, -1);
            foreach (var f in cumulus)
            {
                double xpos = XPosition(f.Item2);
                double ypos = canvasHeight - (f.Item1 * canvasHeight / cumv);
                if (last.X >= 0)
                {
                    Line a = new Line() { X1 = last.X, X2 = xpos, Y1 = last.Y, Y2 = last.Y, StrokeThickness = 1.5, Stroke = Brushes.DarkSeaGreen };
                    ChartCanvas.Children.Add(a);
                    Line b = new Line() { X1 = xpos, X2 = xpos, Y1 = last.Y, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.DimGray };
                    ChartCanvas.Children.Add(b);
                }
                last = new Point(xpos, ypos);
            }

            // draw a horizontal line at each 500K
            _signs.Clear();
            for (double km = 0; km < 100000; km += 500)
            {
                double ypos = canvasHeight - (km * canvasHeight / cumv);
                if ((ypos > 0) && (ypos < canvasHeight))
                {
                    Line l = new Line() { X1 = 4, X2 = _canvasWidth - 4, Y1 = ypos, Y2 = ypos, StrokeThickness = 1, Stroke = Brushes.Black, Opacity = 0.3 };
                    ChartCanvas.Children.Add(l);

                    TextBlock tl = new TextBlock() { Text = $" {km} km ", Foreground = Brushes.Brown, Background = Brushes.Ivory };
                    _signs.Add(tl);
                    Canvas.SetTop(tl, ypos - 8);
                    Canvas.SetLeft(tl, 100);
                    ChartCanvas.Children.Add(tl);
                }
            }

        }

        private void DisplayDateFramework()
        {
            double canvasHeight = ChartCanvas.ActualHeight;

            // List<Balade> riderBallades = _history.TripList;
            ChartCanvas.Children.Clear();
            _minDate =_history.EarliestRide;
            _maxDate = _history.LatestRide;
            int dateInterval = _maxDate.DayNumber - _minDate.DayNumber;
            double dayspan = dateInterval + 1;
            ChartCanvas.Width = _canvasWidth = _xInterval * dayspan;
            DateOnly p = _minDate;
            do
            {
                if (p.Day == 1)
                {
                    double xpos = XPosition(p);
                    Line monthstarter = new Line() { X1 = xpos, X2 = xpos, Y1 = 0, Y2 = canvasHeight, Stroke = Brushes.Ivory, StrokeThickness = 0.8, StrokeDashArray = new DoubleCollection { 6, 3 } };
                    ChartCanvas.Children.Add(monthstarter);
                    TextBlock monthstarterblock = new TextBlock() { Text = p.ToString("MMM yyyy"), Foreground = Brushes.Ivory };
                    Canvas.SetLeft(monthstarterblock, xpos + 4);
                    Canvas.SetTop(monthstarterblock, 8);
                    ChartCanvas.Children.Add(monthstarterblock);
                }
                p = p.AddDays(1);
            } while (p < _maxDate);

            ChartScrollViewer.ScrollToRightEnd();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
       

        private void DailyButton_Click(object sender, RoutedEventArgs e)
        {
            DayStatsWindow w = new DayStatsWindow(_history) { Owner = this };
            w.ShowDialog();
            if (w.DataAltered)
            {
                RefreshDisplay();
            }
        }

        private void RatiosButton_Click(object sender, RoutedEventArgs e)
        {
            RatiosWindow w = new RatiosWindow() { Owner = this };
            w.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _history.SaveData();
        }

        private void WeeklyButton_Click(object sender, RoutedEventArgs e)
        { 
            PeriodStatsWindow w = new PeriodStatsWindow(_history)
            {
                Owner = this
            };
            w.ShowDialog();
        }

        private void ChartScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            foreach (TextBlock bloc in _signs)
            {
                Canvas.SetLeft(bloc, 100 + e.HorizontalOffset);
            }
        }

        private void PlotButton_Click(object sender, RoutedEventArgs e)
        {
            if (_plot == 0)
            {
                _plot = 1;
                DisplayDateFramework();
                DisplayCumulativeGraph();
                PlotTextBlock.Text = "Cumulative distance";
                PlotButton.Content = "Daily rides";
                PlotButton.ToolTip = "Display individual ride distances";
            }
            else
            {
                _plot = 0;
                DisplayDateFramework();
                DisplayRidesGraph();
                PlotTextBlock.Text = "Daily rides";
                PlotButton.Content = "Cumulative distance";
                PlotButton.ToolTip = "Display cumulative distance ridden";
            }
        }

        private void DistanceButton_Click(object sender, RoutedEventArgs e)
        {
            DistanceWindow w = new DistanceWindow(_history) { Owner = this };
            w.ShowDialog();
        }

}