using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Santay;

public partial class GymWindow
{
    private readonly GymHistory _history;
    private readonly DateTime _antedate = new DateTime(2019, 12, 15);
    private readonly double daywidth = 5;

    public GymWindow()
    {
        InitializeComponent();
        _history = new GymHistory("Data.gym");
    }

    private Brush ActivityBrush(GymHistory.GymType g)
    {
        switch (g)
        {
            case GymHistory.GymType.AquaAerobics:
            {
                return Brushes.RoyalBlue;
            }
            case GymHistory.GymType.GymTraining:
            {
                return Brushes.SaddleBrown;
            }
            case GymHistory.GymType.Other:
            {
                return Brushes.SeaGreen;
            }
            default:
            {
                return Brushes.Black;
            }
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        double scrX = SystemParameters.PrimaryScreenWidth;
        double scrY = SystemParameters.PrimaryScreenHeight;
        double winX = scrX * .98;
        double winY = scrY * .9;
        double xm = (scrX - winX) / 2;
        double ym = (scrY - winY) / 4;
        Width = winX;
        Height = winY;
        Left = xm;
        Top = ym;

        
        RefreshList();
        AddButton.IsEnabled = true;
        AddStackPanel.Visibility = Visibility.Hidden;
        CloseButton.IsEnabled = true;
    }

    private void RefreshList()
    {
        DeleteButton.IsEnabled = false;
        _history.GymList.Sort();
        int b = 0;
        foreach (GymVisit g in _history.GymList)
        {
            g.Index = b;
            b++;
        }

        // Display list of visits
        GymVisitsListBox.Items.Clear();
        YesterWeekListBox.Items.Clear();
        TodayWeekListBox.Items.Clear();
        DateTime yesterday = DateTime.Today.AddDays(-1);
        DateTime weekAgoToday = DateTime.Today.AddDays(-6);
        DateTime weekAgoYesterday = DateTime.Today.AddDays(-7);

        List<GymVisit> sevenDaysToYesterday = new List<GymVisit>();
        List<GymVisit> sevenDaysToToday = new List<GymVisit>();

        // List all dates and create lists for week to yesterday and week to today
        foreach (GymVisit g in _history.GymList)
        {
            GymVisitsListBox.Items.Add(ListEntry(g));

            if (g.When >= weekAgoToday)
            {
                sevenDaysToToday.Add(g);
            }

            if ((g.When >= weekAgoYesterday) && (g.When <= yesterday))
            {
                sevenDaysToYesterday.Add(g);
            }
        }

        // List the most recent week

        YesterWeekTextBlock.Text = "Week to yesterday";
        YesterWeekTextBlock.Inlines.Add(Ticky(sevenDaysToYesterday.Count));
        DateTime daymark = weekAgoYesterday;
        while (daymark.Date <= yesterday.Date)
        {
            string mark = GymVisit.StringFromDate(daymark);
            // Don't forget to allow for more than one visit on a given day e.g. gym in the morning, aqua in the evening
            string stuff = string.Empty;
            foreach (GymVisit g in sevenDaysToYesterday)
            {
                if (g.WhenCode == mark)
                {
                    if (g.Activity == GymHistory.GymType.AquaAerobics)
                    {
                        stuff += "A";
                    }

                    if (g.Activity == GymHistory.GymType.GymTraining)
                    {
                        stuff += "G";
                    }

                    if (g.Activity == GymHistory.GymType.Other)
                    {
                        stuff += "O";
                    }
                }
            }

            YesterWeekListBox.Items.Add(ListEntry(daymark, stuff));
            daymark = daymark.AddDays(1);
        }

        TodayWeekTextBlock.Text = "Week to today";
        TodayWeekTextBlock.Inlines.Add(Ticky(sevenDaysToToday.Count));
        daymark = weekAgoToday;
        while (daymark.Date <= DateTime.Today.Date)
        {
            string mark = GymVisit.StringFromDate(daymark);
            string stuff = string.Empty;
            foreach (GymVisit g in sevenDaysToToday)
            {
                if (g.WhenCode == mark)
                {
                    if (g.Activity == GymHistory.GymType.AquaAerobics)
                    {
                        stuff += "A";
                    }

                    if (g.Activity == GymHistory.GymType.GymTraining)
                    {
                        stuff += "G";
                    }

                    if (g.Activity == GymHistory.GymType.Other)
                    {
                        stuff += "O";
                    }
                }
            }

            TodayWeekListBox.Items.Add(ListEntry(daymark, stuff));
            daymark = daymark.AddDays(1);
        }

        // Display all-time, rolling month and rolling week stats
        WeekPanel.Children.Clear();
        MnthPanel.Children.Clear();
        TotlPanel.Children.Clear();

        int wv = PeriodVisitsUpTo(DateTime.Today, 7, out int wa, out int wo);
        TextBlock bloc = new TextBlock()
        {
            Text = "Rolling week sessions", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260
        };
        WeekPanel.Children.Add(bloc);

        bloc = new TextBlock()
        {
            Text = $"{wv} activities in 7 days", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0)
        };
        WeekPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{wv - (wa + wo)} gym", Foreground = ActivityBrush(GymHistory.GymType.GymTraining)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        WeekPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{wa} aqua", Foreground = ActivityBrush(GymHistory.GymType.AquaAerobics)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        WeekPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{wo} other", Foreground = ActivityBrush(GymHistory.GymType.Other)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        WeekPanel.Children.Add(bloc);

        double elapsed = (DateTime.Today - _antedate).TotalDays;
        double twentyeight = Math.Min(28, elapsed);

        int mv = PeriodVisitsUpTo(DateTime.Today, 28, out int ma, out int mo);
        bloc = new TextBlock()
        {
            Text = "Rolling 4-weeks average sessions per week", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260
        };
        MnthPanel.Children.Add(bloc);
        string pc = PercentageString(mv, twentyeight);
        bloc = new TextBlock()
        {
            Text = $"{mv} activities in {twentyeight} days", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0)
        };
        MnthPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{mv - (ma + mo)} gym", Foreground = ActivityBrush(GymHistory.GymType.GymTraining)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        MnthPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{ma} aqua", Foreground = ActivityBrush(GymHistory.GymType.AquaAerobics)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        MnthPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{mo} other", Foreground = ActivityBrush(GymHistory.GymType.Other)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        MnthPanel.Children.Add(bloc);
        bloc = new TextBlock()
            {Text = pc, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)};
        MnthPanel.Children.Add(bloc);

        int v = AllVisitsUpTo(DateTime.Today, out int a, out int o);
        bloc = new TextBlock()
        {
            Text = "All time average sessions per week", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0), FontWeight = FontWeights.Bold, Width = 260
        };
        TotlPanel.Children.Add(bloc);
        pc = PercentageString(v, elapsed);
        bloc = new TextBlock()
        {
            Text = $"{v} activities in {elapsed} days", VerticalAlignment = VerticalAlignment.Center
            , Margin = new Thickness(8, 0, 0, 0)
        };
        TotlPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{v - (a + o)} gym", Foreground = ActivityBrush(GymHistory.GymType.GymTraining)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        TotlPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{a} aqua", Foreground = ActivityBrush(GymHistory.GymType.AquaAerobics)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        TotlPanel.Children.Add(bloc);
        bloc = new TextBlock()
        {
            Text = $"{o} other", Foreground = ActivityBrush(GymHistory.GymType.Other)
            , VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)
        };
        TotlPanel.Children.Add(bloc);
        bloc = new TextBlock()
            {Text = pc, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0)};
        TotlPanel.Children.Add(bloc);

        // Plot sessions and graphs
        PlotSessions();
        PlotGraph();
    }

    private int AllVisitsUpTo(DateTime d, out int a, out int o)
    {
        int v = a = o = 0;
        DateTime lendemain = d.AddDays(1).Date;
        foreach (GymVisit g in _history.GymList)
        {
            if (g.When < lendemain)
            {
                v++;
                if (g.Activity == GymHistory.GymType.AquaAerobics)
                {
                    a++;
                }

                if (g.Activity == GymHistory.GymType.Other)
                {
                    o++;
                }
            }
        }

        return v;
    }

    private int PeriodVisitsUpTo(DateTime d, int days, out int a, out int o)
    {
        int v = a = o = 0;
        DateTime lendemain = d.AddDays(1).Date;
        DateTime baseline = d.AddDays(-days).Date;
        foreach (GymVisit g in _history.GymList)
        {
            if (g.When < lendemain)
            {
                if (g.When > baseline)
                {
                    v++;
                    if (g.Activity == GymHistory.GymType.AquaAerobics)
                    {
                        a++;
                    }

                    if (g.Activity == GymHistory.GymType.Other)
                    {
                        o++;
                    }
                }
            }
        }

        return v;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _history.SaveData();
        DialogResult = true;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddButton.IsEnabled = false;
        DeleteButton.IsEnabled = false;
        CloseButton.IsEnabled = false;
        AddStackPanel.Visibility = Visibility.Visible;
        GymDatePicker.DisplayDateStart = new DateTime(2019, 12, 1);
        GymDatePicker.DisplayDateEnd = DateTime.Today;
        GymDatePicker.SelectedDate = DateTime.Today;
        OtherRadio.IsChecked = true;
    }

    private void AddCancelButton_Click(object sender, RoutedEventArgs e)
    {
        AddButton.IsEnabled = true;
        DeleteButton.IsEnabled = false;
        CloseButton.IsEnabled = true;
        AddStackPanel.Visibility = Visibility.Hidden;
    }

    private void AddConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        GymVisit gv = new GymVisit();
        if (GymRadio.IsChecked ?? false)
        {
            gv.Activity = GymHistory.GymType.GymTraining;
        }
        else if (AquaRadio.IsChecked ?? false)
        {
            gv.Activity = GymHistory.GymType.AquaAerobics;
        }
        else if (OtherRadio.IsChecked ?? false)
        {
            gv.Activity = GymHistory.GymType.Other;
        }

        if (GymDatePicker.SelectedDate.HasValue)
        {
            gv.When = GymDatePicker.SelectedDate.Value;
            if (gv.When > DateTime.Today)
            {
                MessageBox.Show("The selected date is in the future", Jbh.AppManager.AppName, MessageBoxButton.OK
                    , MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            MessageBox.Show("Please select a date", Jbh.AppManager.AppName, MessageBoxButton.OK
                , MessageBoxImage.Warning);
            return;
        }

        _history.GymList.Add(gv);
        RefreshList();
        AddButton.IsEnabled = true;
        CloseButton.IsEnabled = true;
        AddStackPanel.Visibility = Visibility.Hidden;
    }

    private void GymVisitsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool q = GymVisitsListBox.SelectedIndex >= 0;
        DeleteButton.IsEnabled = q;
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (GymVisitsListBox.SelectedItem is ListBoxItem {Tag: int i})
        {
            _history.GymList.RemoveAt(i);
            RefreshList();        
        }
    }

    private void PlotSessions()
    {
        GraphCanvasSess.Children.Clear();
        DateTime moment = new DateTime(2019, 12, 16);
        int daysago = (int) Math.Floor((DateTime.Today - moment).TotalDays);
        //double fourLeft = daywidth * (daysago - 25);
        //double weekLeft = daywidth * (daysago - 4);
        double fourLeft = daywidth * (daysago - 27);
        double weekLeft = daywidth * (daysago - 6);
        Rectangle fourrect = new Rectangle()
            {Width = daywidth * 28, Height = GraphCanvasSess.ActualHeight, Fill = Brushes.BurlyWood, Opacity = 0.4};
        Rectangle weekrect = new Rectangle()
            {Width = daywidth * 7, Height = GraphCanvasSess.ActualHeight, Fill = Brushes.BurlyWood, Opacity = 0.7};
        Canvas.SetLeft(fourrect, fourLeft);
        Canvas.SetLeft(weekrect, weekLeft);
        GraphCanvasSess.Children.Add(fourrect);
        GraphCanvasSess.Children.Add(weekrect);
        TextBlock explain4Tb = new TextBlock() {Text = "28 days", FontSize = 10, Foreground = Brushes.SaddleBrown};
        Canvas.SetLeft(explain4Tb, fourLeft);
        Canvas.SetTop(explain4Tb, 30);
        GraphCanvasSess.Children.Add(explain4Tb);
        TextBlock explain1Tb = new TextBlock() {Text = "7 days", FontSize = 10, Foreground = Brushes.SaddleBrown};
        Canvas.SetLeft(explain1Tb, weekLeft);
        Canvas.SetTop(explain1Tb, 30);
        GraphCanvasSess.Children.Add(explain1Tb);
        //double xpos = daywidth;
        double xpos = 0;
        while (moment <= DateTime.Today)
        {
            xpos += daywidth;
            string code = GymVisit.StringFromDate(moment);
            List<GymVisit> daySessions = new List<GymVisit>();

            foreach (GymVisit gv in _history.GymList)
            {
                if (gv.WhenCode == code)
                {
                    daySessions.Add(gv);
                }
            }

            switch (daySessions.Count)
            {
                case 0:
                {
                    Rectangle carre = new Rectangle()
                    {
                        Height = 24, Width = daywidth - 0.5, Fill = Brushes.White, Stroke = Brushes.Tan
                        , StrokeThickness = 0.3
                    };
                    Canvas.SetLeft(carre, xpos);
                    Canvas.SetTop(carre, 4);
                    GraphCanvasSess.Children.Add(carre);
                    break;
                }
                case 1:
                {
                    double ypos = 10;
                    GymVisit gv = daySessions[0];

                    Brush pinceau = ActivityBrush(gv.Activity);
                    Rectangle carre = new Rectangle() {Height = 12, Width = daywidth - 0.5, Fill = pinceau};
                    Canvas.SetLeft(carre, xpos);
                    Canvas.SetTop(carre, ypos);
                    GraphCanvasSess.Children.Add(carre);

                    break;
                }
                default:
                {
                    double ypos = 4;
                    double visits = daySessions.Count;
                    double sessionHeight = (24 - (2 * (visits - 1))) / visits;
                    foreach (GymVisit gv in daySessions)
                    {
                        Brush pinceau = ActivityBrush(gv.Activity);
                        Rectangle carre = new Rectangle()
                            {Height = sessionHeight, Width = daywidth - 0.5, Fill = pinceau};
                        Canvas.SetLeft(carre, xpos);
                        Canvas.SetTop(carre, ypos);
                        GraphCanvasSess.Children.Add(carre);
                        ypos += (sessionHeight + 2);
                    }

                    break;
                }
            }

            moment = moment.AddDays(1);
        }
    }

    private void PlotGraph()
    {
        double gcw = GraphCanvasWeek.ActualWidth;
        double gch = GraphCanvasWeek.ActualHeight;

        double yHeight = gch / 7.1;

        GraphCanvasWeek.Children.Clear();
        GraphCanvasMnth.Children.Clear();
        GraphCanvasTotl.Children.Clear();

        // Draw horizontal interval lines
        for (int f = 1; f < 8; f++)
        {
            double yp = gch - (f * yHeight);
            if (f == 5)
            {
                Line w = new Line()
                {
                    X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5
                    , StrokeDashArray = new DoubleCollection {12, 6}
                };
                GraphCanvasWeek.Children.Add(w);
                Line m = new Line()
                {
                    X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5
                    , StrokeDashArray = new DoubleCollection {12, 6}
                };
                GraphCanvasMnth.Children.Add(m);
                Line t = new Line()
                {
                    X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.SaddleBrown, StrokeThickness = 0.5
                    , StrokeDashArray = new DoubleCollection {12, 6}
                };
                GraphCanvasTotl.Children.Add(t);
            }
            else
            {
                Line w = new Line()
                    {X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5};
                GraphCanvasWeek.Children.Add(w);
                Line m = new Line()
                    {X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5};
                GraphCanvasMnth.Children.Add(m);
                Line t = new Line()
                    {X1 = 0, X2 = gcw, Y1 = yp, Y2 = yp, Stroke = Brushes.BurlyWood, StrokeThickness = 0.5};
                GraphCanvasTotl.Children.Add(t);
            }
        }

        DateTime moment = new DateTime(2019, 12, 16);

        // Get values
        List<Tuple<DateTime, double>> weekliesG = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> weekliesGa = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> weekliesGao = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> mnthliesG = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> mnthliesGa = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> mnthliesGao = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> totlliesG = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> totlliesGa = new List<Tuple<DateTime, double>>();
        List<Tuple<DateTime, double>> totlliesGao = new List<Tuple<DateTime, double>>();

        while (moment <= DateTime.Today)
        {
            int wv = PeriodVisitsUpTo(moment, 7, out int wa, out int wo);
            int mv = PeriodVisitsUpTo(moment, 28, out int ma, out int mo);
            int tv = AllVisitsUpTo(moment, out int ta, out int to);

            double elapsed = (moment - _antedate).TotalDays;
            double twentyeight = Math.Min(28, elapsed);
            double mnthweeks = 4 * (twentyeight / 28);
            double totlweeks = elapsed / 7;
            Tuple<DateTime, double> wG = new Tuple<DateTime, double>(moment, wv - (wa + wo));
            Tuple<DateTime, double> wGa = new Tuple<DateTime, double>(moment, wv - wo);
            Tuple<DateTime, double> wGao = new Tuple<DateTime, double>(moment, wv);
            Tuple<DateTime, double> mG = new Tuple<DateTime, double>(moment, (mv - (ma + mo)) / mnthweeks);
            Tuple<DateTime, double> mGa = new Tuple<DateTime, double>(moment, (mv - mo) / mnthweeks);
            Tuple<DateTime, double> mGao = new Tuple<DateTime, double>(moment, mv / mnthweeks);
            Tuple<DateTime, double> tG = new Tuple<DateTime, double>(moment, (tv - (ta + to)) / totlweeks);
            Tuple<DateTime, double> tGa = new Tuple<DateTime, double>(moment, (tv - to) / totlweeks);
            Tuple<DateTime, double> tGao = new Tuple<DateTime, double>(moment, tv / totlweeks);
            weekliesG.Add(wG);
            weekliesGa.Add(wGa);
            weekliesGao.Add(wGao);
            mnthliesG.Add(mG);
            mnthliesGa.Add(mGa);
            mnthliesGao.Add(mGao);
            totlliesG.Add(tG);
            totlliesGa.Add(tGa);
            totlliesGao.Add(tGao);
            moment = moment.AddDays(1);
        }

        //// Plot graph
        int duration = weekliesGao.Count;
        double cwidth = Math.Max(gcw, duration * daywidth);
        GraphCanvasSess.Width = cwidth;
        GraphCanvasWeek.Width = cwidth;
        GraphCanvasMnth.Width = cwidth;
        GraphCanvasTotl.Width = cwidth;
        SessScroller.ScrollToRightEnd();
        WeekScroller.ScrollToRightEnd();
        MnthScroller.ScrollToRightEnd();
        TotlScroller.ScrollToRightEnd();

        PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.Other), weekliesGao);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.AquaAerobics)
            , weekliesGa);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasWeek, ActivityBrush(GymHistory.GymType.GymTraining)
            , weekliesG);

        PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.Other), mnthliesGao);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.AquaAerobics)
            , mnthliesGa);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasMnth, ActivityBrush(GymHistory.GymType.GymTraining)
            , mnthliesG);

        PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.Other), totlliesGao);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.AquaAerobics)
            , totlliesGa);
        PlotPerformance(daywidth, gch, yHeight, GraphCanvasTotl, ActivityBrush(GymHistory.GymType.GymTraining)
            , totlliesG);
    }

    private void PlotPerformance(double dayWidth, double graphHeight, double incrementHeight, Canvas slate
        , Brush pinceau, List<Tuple<DateTime, double>> data)
    {
        double xpos = 0;
        //int counter = data.Count;
        foreach (Tuple<DateTime, double> tup in data)
        {
            xpos += dayWidth;
            double ypos = graphHeight - (incrementHeight * tup.Item2);
            //Point next = new Point() {X = xpos, Y = ypos};
            Rectangle rex = new Rectangle()
                {Fill = pinceau, Width = dayWidth, Height = incrementHeight * tup.Item2, Opacity = 0.7};
            Canvas.SetLeft(rex, xpos);
            Canvas.SetTop(rex, ypos);
            slate.Children.Add(rex);
            //counter--;
        }
    }

    private ListBoxItem ListEntry(GymVisit gv)
    {
        FontFamily ff = new FontFamily("Lucida Console");
        string which = "Nothing";
        Brush brosse = ActivityBrush(gv.Activity);
        if (gv.Activity == GymHistory.GymType.AquaAerobics)
        {
            which = "Aqua aerobics";
        }
        else if (gv.Activity == GymHistory.GymType.GymTraining)
        {
            which = "Gym training";
        }
        else if (gv.Activity == GymHistory.GymType.Other)
        {
            which = "Other activity";
        }

        TextBlock td = new TextBlock() {Text = $"{gv.When.ToShortDateString()} {gv.When:ddd}", FontFamily = ff};
        TextBlock tw = new TextBlock()
            {Text = which, FontFamily = ff, Foreground = brosse, Margin = new Thickness(8, 0, 0, 0)};
        StackPanel sp = new StackPanel() {Orientation = Orientation.Horizontal};
        sp.Children.Add(td);
        sp.Children.Add(tw);
        return new ListBoxItem() {Content = sp, Tag = gv.Index, IsHitTestVisible = (gv.Index > 0)};
    }

    private ListBoxItem ListEntry(DateTime when, string what)
    {
        FontFamily ff = new FontFamily("Lucida Console");
        Thickness marge = new Thickness(6, 0, 0, 3);
        Brush brosse = Brushes.BurlyWood;

        TextBlock td = new TextBlock() {Text = $"{when:ddd}", FontFamily = ff};
        StackPanel sp = new StackPanel() {Orientation = Orientation.Horizontal};
        sp.Children.Add(td);
        foreach (var q in what)
        {
            if (q == 'A')
            {
                brosse = ActivityBrush(GymHistory.GymType.AquaAerobics);
            }
            else if (q == 'G')
            {
                brosse = ActivityBrush(GymHistory.GymType.GymTraining);
            }
            else if (q == 'O')
            {
                brosse = ActivityBrush(GymHistory.GymType.Other);
            }

            Rectangle oblong = new Rectangle() {Width = 20, Height = 9, Margin = marge, Fill = brosse};
            sp.Children.Add(oblong);
        }

        return new ListBoxItem() {Content = sp, IsHitTestVisible = false};
    }

    private string PercentageString(int num, double tot)
    {
        if (tot == 0)
        {
            return "ERR%";
        }

        double all = tot;
        double per = 70 * (num / all);
        per = Math.Round(per) / 10;
        return $"{per:0.0} per week";
    }

    private Run Ticky(int v)
    {
        string tick = (v >= 5) ? " ü" : " û"; // tick or cross
        Brush brosse = (v >= 5) ? Brushes.Green : Brushes.Red;
        Run couru = new Run()
        {
            Text = tick, FontWeight = FontWeights.Bold, FontFamily = new FontFamily("Wingdings"), FontSize = 18
            , Foreground = brosse
        };
        return couru;
    }

    private void AddTodayButton_Click(object sender, RoutedEventArgs e)
    {
        GymDatePicker.SelectedDate = DateTime.Today;
    }

    private void AddYesterdayButton_Click(object sender, RoutedEventArgs e)
    {
        GymDatePicker.SelectedDate = DateTime.Today.AddDays(-1);
    }

    private void AddAvanthierButton_Click(object sender, RoutedEventArgs e)
    {
        GymDatePicker.SelectedDate = DateTime.Today.AddDays(-2);
    }

    private void AddBackwardButton_Click(object sender, RoutedEventArgs e)
    {
        DateTime when = DateTime.Today;
        if (GymDatePicker.SelectedDate.HasValue)
        {
            when = GymDatePicker.SelectedDate.Value;
        }

        GymDatePicker.SelectedDate = when.AddDays(-1);
    }

    private void AddForwardsButton_Click(object sender, RoutedEventArgs e)
    {
        DateTime when = DateTime.Today;
        if (GymDatePicker.SelectedDate.HasValue)
        {
            when = GymDatePicker.SelectedDate.Value;
        }

        GymDatePicker.SelectedDate = when.AddDays(1);
    }
}