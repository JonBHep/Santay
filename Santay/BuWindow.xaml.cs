using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Jbh;

namespace Santay;

public partial class BuWindow
{
    public BuWindow()
        {
            InitializeComponent();
            _buDictionary = new Dictionary<int, BuDate.Ivresse>();
        }

        private const int BuBeans= 5;
        private const int PasbuBeans = 2;
        private const string Budatafile= "Data.bu";

        private readonly DateTime _startDate = new DateTime(2021, 10, 15);
        private readonly Dictionary<int, BuDate.Ivresse> _buDictionary;

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

            double perweek = 7 * PasbuBeans / (double)(BuBeans + PasbuBeans);

            RubricTextBlock.Text = $"For every day on which I don't drink, {BeanString(PasbuBeans)} {AreString(PasbuBeans)} put into the pot. For every day on which I do drink, {BeanString(BuBeans)} {AreString(BuBeans)} taken out of the pot. The aim is to keep the number of beans in the pot positive. {perweek} drinking days per week will keep the beans steady.";
            CycleBorder.Visibility = Visibility.Hidden;
            LoadData();
        }
        
        private string BeanString(int b)
        {
            string r = "beans";
            if (b == 1 || b == -1) { r = "bean"; }
            return $"{b} {r}";
        }

        private string AreString(int b)
        {
            string r = "are";
            if (b == 1 || b == -1) { r = "is"; }
            return r;
        }

        private void SaveData()
        {
            string bufile = System.IO.Path.Combine(AppManager.DataPath, Budatafile);
            // backup existing data
            AppManager.CreateBackupDataFile(bufile);
            AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(Budatafile), minimumDaysToKeep: 40, minimumFilesToKeep: 4);

            // write new data
            using System.IO.StreamWriter sw = new System.IO.StreamWriter(bufile);
            foreach (int d in _buDictionary.Keys)
            {
                BuDate bud = new BuDate() { QuandInteger = d, Quoi = _buDictionary[d] };
                sw.WriteLine(bud.Specification);
            }
        }

        private void LoadData()
        {
            InitialiseDictionary();
            string bufile = System.IO.Path.Combine(AppManager.DataPath, Budatafile);
            if (System.IO.File.Exists(bufile))
            {
                using System.IO.StreamReader rd = new System.IO.StreamReader(bufile);
                while (!rd.EndOfStream)
                {
                    var i = rd.ReadLine();
                    if (i is { })
                    {
                        BuDate bd = new BuDate() {Specification = i};
                        int j = bd.QuandInteger;
                        if (_buDictionary.ContainsKey(j))
                        {
                            _buDictionary[j] = bd.Quoi;
                        }
                        else
                        {
                            _buDictionary.Add(j
                                , bd.Quoi); // theoretically, this should not occur as dictionary has been initialised with range of dates
                        }
                    }
                }
            }

            RefreshDateList();
        }

        private void InitialiseDictionary()
        {
            _buDictionary.Clear();
            int jour = BuDate.DayOfLife(_startDate);
            int maintenant = BuDate.DayOfLife(DateTime.Today);
            while (jour <= maintenant)
            {
                _buDictionary.Add(jour, BuDate.Ivresse.SaisPas);
                jour++;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }

        private void RefreshDateList()
        {
            int f = DatesListBox.SelectedIndex;
            int beans = 0;
            DatesListBox.Items.Clear();
            List<int> days = new List<int>();
            foreach (int j in _buDictionary.Keys)
            {
                days.Add(j);
            }
            days.Sort();
            foreach (int j in days)
            {
                StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                TextBlock bloc = new TextBlock() { Text = BuDate.DateStamp(j), FontFamily = new FontFamily("Consolas"),VerticalAlignment = VerticalAlignment.Center };
                TextBlock sign = new TextBlock() {Width=32 , VerticalAlignment= VerticalAlignment.Center, TextAlignment= TextAlignment.Center };
                beans = BeansAfter(beans, _buDictionary[j]);
                switch (_buDictionary[j])
                {
                    case BuDate.Ivresse.SaisPas: { sign.Text= "s"; sign.FontFamily = new FontFamily("Webdings");sign.FontSize = 14; sign.Foreground = Brushes.Red; break; }
                    case BuDate.Ivresse.Bu: { sign.Text = "S"; sign.FontFamily = new FontFamily("Wingdings"); sign.FontSize = 16; sign.Foreground = Brushes.BlueViolet; break; }
                    case BuDate.Ivresse.PasBu: { sign.Text = "r"; sign.FontFamily = new FontFamily("Webdings"); sign.FontSize = 14; sign.Foreground = Brushes.ForestGreen; break; }
                }
                panel.Children.Add(bloc);
                panel.Children.Add(sign);
                panel.Children.Add(new TextBlock() { Text = $"{beans} beans", Foreground = beans < 0 ? Brushes.Red : Brushes.DarkGreen, VerticalAlignment= VerticalAlignment.Center, FontFamily = new FontFamily("Consolas") });
                DatesListBox.Items.Insert(0,new ListBoxItem() {Tag= j, Height=24, Content = panel} );
            }
            DatesListBox.SelectedIndex = f;
            DatesListBox.ScrollIntoView(DatesListBox.SelectedItem);
            int interval = BuDate.DayOfLife(DateTime.Today) - LastDrinkIndex();
            LastTextBlock.Text =$"Last drink {BuDate.DateStamp(LastDrinkIndex())} ({interval} days)";
            RefreshGraphics();
        }
        
        private int LastDrinkIndex()
        {
            int last = 0;
            foreach (int j in _buDictionary.Keys)
            {
                if (_buDictionary[j] != BuDate.Ivresse.Bu) continue;
                if (j > last)
                {
                    last = j;
                }
            }

            return last;
        }

        private void RefreshGraphics()
        {
            // Show graphics for periods up to today if today's result is known, otherwise to yesterday
            DateTime lastDate = DateTime.Today;
            int lastDol = BuDate.DayOfLife(lastDate);
            UntilTextBlock.Text = "Graphics up to TODAY";
            if (_buDictionary[lastDol] == BuDate.Ivresse.SaisPas)
            {
                lastDate = DateTime.Today.AddDays(-1);
                lastDol = BuDate.DayOfLife(lastDate);
                UntilTextBlock.Text = "Graphics up to YESTERDAY";
            }

            double elapsedDays = (lastDate - _startDate).TotalDays+1;
            double elapsedweeks = elapsedDays / 7;
            PlotRecentDays(lastDol);
            
            int z = PlotRollingPeriod(WeekCanvas, 7, elapsedDays, lastDol);
            WeeklyBalanceTextBlock.Text = $"{z}";
            
            z = PlotRollingPeriod(MonthCanvas, 28, elapsedDays, lastDol);
            MonthlyBalanceTextBlock.Text = $"{z} ({z / 4d} per week)";
            
            z = PlotRollingPeriod(SixMonthCanvas, 182, elapsedDays, lastDol);
            SixMonthlyBalanceTextBlock.Text = $"{z} ({z/26d:0.0} per week)";
            
            z = PlotRollingPeriod(TotalCanvas, 99999, elapsedDays, lastDol);
            TotalBalanceTextBlock.Text = $"{z} ({z/elapsedweeks:0.0} per week)";
        }

        private void PlotRecentDays(int finalCode)
        {
            RecentDaysDockPanel.Children.Clear();
            int currentcode = finalCode + 1;
            for (int z=0; z<77; z++)
            {
                currentcode--;
                if (_buDictionary.ContainsKey(currentcode))
                {
                    Polygon bouteille = Bottle(_buDictionary[currentcode]);
                    DockPanel.SetDock(bouteille, Dock.Right);
                    RecentDaysDockPanel.Children.Add(bouteille);

                    if (z % 7 == 6)
                    {
                        // add week marker
                        Ellipse elyps=new Ellipse() { Width = 16, Height = 16, Fill = Brushes.Tan, Margin = new Thickness(1, 2, 1, 2) };
                        DockPanel.SetDock(elyps, Dock.Right);
                        RecentDaysDockPanel.Children.Add(elyps);
                    }
                }
            }
        }

        private Polygon Bottle(BuDate.Ivresse etat)
        {
            Brush pinceau;
            Brush outline;
            switch (etat)
            {
                case BuDate.Ivresse.Bu: { pinceau = Brushes.Firebrick; outline = Brushes.Brown; break; }
                case BuDate.Ivresse.PasBu: { pinceau = Brushes.Aquamarine; outline = Brushes.Black; break; }
                default: { pinceau = Brushes.Silver; outline = Brushes.Gainsboro; break; }
            }

            Polygon myPolygon = new Polygon
            {
                Stroke = outline,
                Fill = pinceau,
                StrokeThickness = 1,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 4, 0)
            };
            PointCollection myPointCollection = new PointCollection
                    {
                        new Point(0, 24),
                        new Point(7, 24),
                        new Point(7, 7),
                        new Point(4.67, 6),
                        new Point(4.67, 0),
                        new Point(2.33, 0),
                        new Point(2.33, 6),
                        new Point(0, 7)
                    };
            myPolygon.Points = myPointCollection;
            return myPolygon;
        }

        private int PlotRollingPeriod(Canvas whichCanvas, int spanDays, double totalDays, int endDateCode)
        {
            whichCanvas.Children.Clear();
            double cheight = whichCanvas.ActualHeight;
            double topmargin = 8;
            double usableheight = cheight - (2*topmargin);

            double wks = Math.Round(totalDays / 7);
            AllTimeCaptionBloc.Text = $"All time ({wks} weeks) bean balance";
            double dayInterval = 4;
            whichCanvas.Width = dayInterval * totalDays;
            (int, int) minimax = PeriodMinMaxBeans(spanDays);
            double tspan = minimax.Item2 - minimax.Item1;
            double yratio = usableheight / tspan;
            double prop = minimax.Item2 / tspan;
            double yorigin =topmargin+ usableheight * prop;
            whichCanvas.Children.Add(new Line() { X1 = 0, X2 = dayInterval * totalDays, Y1 = yorigin, Y2 = yorigin, StrokeThickness = 1.5, Stroke = Brushes.CornflowerBlue, StrokeDashArray = { 5, 3 } });
            int firstdate = _buDictionary.Keys.First();

            double xposition = 0;
            double lastx = 0;
            double lasty = yorigin;
            int beans = 0;
            foreach (int target in _buDictionary.Keys)
            {
                if (target <= endDateCode) // if last date is yesterday do not include today in graph
                {
                    xposition += dayInterval;
                    beans = 0;
                    int debut = target - (spanDays - 1);
                    if (debut < firstdate) { debut = firstdate; }
                    for (int x = debut; x <= target; x++)
                    {
                        beans = BeansAfter(beans, _buDictionary[x]);
                    }
                    double thisY = yorigin - (beans * yratio);
                    whichCanvas.Children.Add(new Line() { X1 = lastx, Y1 = lasty, X2 = xposition, Y2 = thisY, Stroke = Brushes.OrangeRed, StrokeThickness = 1.5 });

                    lastx = xposition;
                    lasty = thisY;
                }
            }
            return beans;
        }

        private (int, int) PeriodMinMaxBeans(int days)
        {
            int mini = 0;
            int maxi = 0;
            int firstdate = _buDictionary.Keys.First();
            foreach (int target in _buDictionary.Keys)
            {
                int xbeans = 0;
                int debut = target - (days-1);
                if (debut < firstdate) { debut = firstdate; }
                for (int x = debut; x <= target; x++)
                {
                    xbeans = BeansAfter(xbeans, _buDictionary[x]);
                }

                mini = Math.Min(mini, xbeans);
                maxi = Math.Max(maxi, xbeans);
            }
            return (mini, maxi);
        }

        private static int BeansAfter(int feves, BuDate.Ivresse quoi)
        {
            int apres;
            switch (quoi)
            {
                case BuDate.Ivresse.Bu:
                {
                    apres = feves - BuBeans;
                    break;
                }
                case BuDate.Ivresse.PasBu:
                {
                    apres = feves + PasbuBeans;
                    break;
                }
                default:
                {
                    apres = feves;
                    break;
                }
            }

            return apres;
        }

        private void CycleButton_Click(object sender, RoutedEventArgs e)
        {
            if (DatesListBox.SelectedItem is ListBoxItem {Tag: int i})
            {
                BuDate.Ivresse ivre = _buDictionary[i];
                BuDate.Ivresse nova = BuDate.Ivresse.SaisPas;
                switch (ivre)
                {
                    case BuDate.Ivresse.SaisPas: { nova = BuDate.Ivresse.Bu; break; }
                    case BuDate.Ivresse.Bu: { nova = BuDate.Ivresse.PasBu; break; }
                    case BuDate.Ivresse.PasBu: { nova = BuDate.Ivresse.SaisPas; break; }
                }
                _buDictionary[i] = nova;
                RefreshDateList();
            }
        }

        private void DatesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CycleBorder.Visibility = (DatesListBox.SelectedIndex < 0) ? Visibility.Hidden : Visibility.Visible;
        }
}