using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Santay;

public partial class DayStatsWindow
{
    private readonly VeloHistory _history;
        bool _altered;

        public DayStatsWindow(VeloHistory history)
        {
            InitializeComponent();
            _history = history;
            _altered = false;
        }

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

        private void DisplayOutings()
        {
            TripsListBox.Items.Clear();

            if (_history.TripList.Count < 1) { return; }

            bool shownDemarcation = false;
            DateOnly demarcation = new DateOnly(2018, 4, 10);
            DateOnly startDate = _history.HistoryFirstDate;
            DateOnly endDate =DateOnly.FromDateTime( DateTime.Today).AddDays(1);

            while (startDate < endDate)
            {
                if (!shownDemarcation && startDate >= demarcation)
                {
                    Rectangle demarc = new Rectangle() { Height = 8, Width = 100, Fill = Brushes.Yellow };
                    TripsListBox.Items.Add(new ListBoxItem() { Content = demarc, IsHitTestVisible = false });
                    shownDemarcation = true;
                }

                Balade? excursion = _history.TripOnDate(startDate);
                
                if (excursion is {})
                {
                    //Balade b = _history.TripOnDate(startDate);
                    Brush pinceau = Brushes.DarkMagenta;
                    Brush pinceau2 = Brushes.MediumSeaGreen;
                    // if (excursion.Kind == VeloHistory.TripType.Walk) { pinceau = Brushes.DarkMagenta; pinceau2 = Brushes.Magenta; }
                    TextBlock dayBlk = new TextBlock() { Text = excursion.TripDate.ToString("ddd dd MMM yyyy"), Width = 108, Foreground = pinceau };

                    TextBlock dstBlkJ = new TextBlock() { Text = excursion.RideKmStringJbh, Width = 60, TextAlignment = TextAlignment.Right, Foreground = Brushes.MediumBlue };

                    string distanceRank = string.Empty;
                    if (excursion.RideKm > 0)
                    {
                        // if (excursion.Kind == VeloHistory.TripType.Cycle)
                        // {
                            int distrank = _history.DistanceRanking(excursion.RideKm);
                            distanceRank = VeloHistory.Ordinal(distrank);
                            if (distrank < 7) { RecordTop5Trip(distrank, excursion); }
                        // }
                    }

                    TextBlock dstRnkBlkJ = new TextBlock() { Text = distanceRank, Width = 76, Foreground = Brushes.MediumBlue, TextAlignment = TextAlignment.Center };

                    TextBlock diffBlk = new TextBlock() { Text = Balade.DifficultyCaption(excursion.Difficulty), Width = 108, Foreground = pinceau };
                    TextBlock epocBlk = new TextBlock() { Text = excursion.RideGroup, Foreground = pinceau2 };
                    TextBlock routBlk = new TextBlock() { Text = excursion.RideCaption, Margin = new Thickness(10, 0, 0, 0), Foreground = pinceau };
                    StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    sp.Children.Add(dayBlk);

                    sp.Children.Add(dstBlkJ);
                    sp.Children.Add(dstRnkBlkJ);

                    sp.Children.Add(diffBlk);
                    sp.Children.Add(epocBlk);
                    sp.Children.Add(routBlk);

                    ListBoxItem itm = new ListBoxItem() { Content = sp, Tag = startDate };
                    TripsListBox.Items.Add(itm);
                }
                startDate = startDate.AddDays(1);
            }
            TripsListBox.ScrollIntoView(TripsListBox.Items[^1]);
        }
                
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Title = "Velo daily statistics";
            DisplayInformation();
        }

        private void DisplayInformation()
        {
            TripCountJTbk.Text = _history.TripCountV.ToString();
            // TripCountJTbkW.Text = _history.TripCountW.ToString();

            double v = _history.TotalDistanceKmV;
            TotKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TotMlJTbk.Text = v.ToString("0.0") + " m";

            // v = _history.TotalDistanceKmW;
            // TotKmJTbkW.Text = v.ToString("0.0") + " km";
            // v = VeloHistory.MilesFromKm(v);
            // TotMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.MaximumTripKmVelo;
            TripMaxKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripMaxMlJTbk.Text = v.ToString("0.0") + " m";

            // v = _history.MaximumTripKmPied;
            // TripMaxKmJTbkW.Text = v.ToString("0.0") + " km";
            // v = VeloHistory.MilesFromKm(v);
            // TripMaxMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.AverageTripKmVelo;
            TripAveKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            TripAveMlJTbk.Text = v.ToString("0.0") + " m";

            // v = _history.AverageTripKmPied;
            // TripAveKmJTbkW.Text = v.ToString("0.0") + " km";
            // v = VeloHistory.MilesFromKm(v);
            // TripAveMlJTbkW.Text = v.ToString("0.0") + " m";

            v = _history.AverageDailyKmVelo;
            DayAveKmJTbk.Text = v.ToString("0.0") + " km";
            v = VeloHistory.MilesFromKm(v);
            DayAveMlJTbk.Text = v.ToString("0.0") + " m";

            // v = _history.AverageDailyKmPied;
            // DayAveKmJTbkW.Text = v.ToString("0.0") + " km";
            // v = VeloHistory.MilesFromKm(v);
            // DayAveMlJTbkW.Text = v.ToString("0.0") + " m";

            DisplayOutings();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Balade b = new Balade(DateTime.Today, 0, string.Empty, string.Empty, 2);
            BaladeWindow bw = new BaladeWindow(b, _history.GroupList) { Owner = this };
            bool? q = bw.ShowDialog();
            if (q is false) { return; }
            Balade s = bw.TripDetails;

            //DateTime dt = s.RideDate;
            DateOnly dto = s.TripDate;
            Balade? outing = _history.TripOnDate(dto);
            if (outing is {})
            {
                MessageBox.Show("Information has already been recorded for this date", "Input error", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }
            _history.AddTrip(s);
            DisplayInformation();
            _altered = true;
        }

        private void EditDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripsListBox.SelectedItem is ListBoxItem {Tag: DateOnly d})
            {
                Balade? ba = _history.TripOnDate(d);
                if (ba is { })
                {
                    string targetSpec = ba.Specification;
                    Balade b = new Balade(targetSpec);

                    BaladeWindow bw = new BaladeWindow(b, _history.GroupList) {Owner = this};
                    bool? q = bw.ShowDialog();
                    if (q is false)
                    {
                        return;
                    }

                    Balade s = bw.TripDetails;

                    if (bw.DateAltered)
                    {
                        //DateTime dt = s.RideDate;
                        DateOnly dto = s.TripDate;
                        Balade? outing = _history.TripOnDate(dto);
                        if (outing is { })
                        {
                            MessageBox.Show("Information has already been recorded for this date", "Input error"
                                , MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            return;
                        }
                    }

                    _history.RemoveTripOnDate(d);
                    _history.AddTrip(s);
                    DisplayInformation();
                    _altered = true;
                }
            }
        }

        private void DeleteDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TripsListBox.SelectedItem is ListBoxItem {Tag: DateOnly d})
            {
                string targetDate = d.ToShortDateString();
                if (MessageBox.Show("Delete ride for " + targetDate, "Delete entry", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel) { return; }
                _history.RemoveTripOnDate(d);
                DisplayInformation();
                _altered = true;
            }
        }

        private void TripsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditDateButton.IsEnabled = DeleteDateButton.IsEnabled = (TripsListBox.SelectedIndex >= 0);
        }

        public bool DataAltered => _altered;

        private void RecordTop5Trip(int rank, Balade bde)
        {
            switch (rank)
            {
                case 1:
                    {
                        Top5Date1TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist1TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route1TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 2:
                    {
                        Top5Date2TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist2TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route2TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 3:
                    {
                        Top5Date3TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist3TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route3TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 4:
                    {
                        Top5Date4TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist4TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route4TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 5:
                    {
                        Top5Date5TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist5TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route5TextBlock.Text = bde.RideCaption;
                        break;
                    }
                case 6:
                    {
                        Top5Date6TextBlock.Text = bde.TripDate.ToString("ddd dd MMM yyyy");
                        Top5Dist6TextBlock.Text = bde.RideKmStringJbh;
                        Top5Route6TextBlock.Text = bde.RideCaption;
                        break;
                    }
            }
        }
}