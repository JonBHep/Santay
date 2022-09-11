using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Santay;

public partial class PrevuWindow
{
    public PrevuWindow()
    {
        InitializeComponent();
        _forecast = new Dictionary<int, PrevuDate>();
    }

    private readonly Dictionary<int, PrevuDate> _forecast;

    private void PurgeEmptyDays()
    {
        var empties = new List<int>();
        foreach (var key in _forecast.Keys)
        {
            if (_forecast[key].Empty){empties.Add(key);}
        }

        foreach (var empty in empties)
        {
            _forecast.Remove(empty);
        }
    } 
    
    private void PrevuWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        var scrX = SystemParameters.PrimaryScreenWidth;
        var scrY = SystemParameters.PrimaryScreenHeight;
        var winX = scrX * .98;
        var winY = scrY * .94;
        var xm = (scrX - winX) / 2;
        var ym = (scrY - winY) / 4;
        Width = winX;
        Height = winY;
        Left = xm;
        Top = ym;
        LoadData();
    }

    private void LoadData()
    {
        var path = Path.Combine(Jbh.AppManager.DataPath, "Data.prevu");
        if (File.Exists(path))
        {
            _forecast.Clear();
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var j = reader.ReadLine();
                if (j is null) continue;
                var log = new PrevuDate() {Specification = j};
                _forecast.Add(log.EntryDate.DayNumber, log);
            }
        }
    }

    private void SaveData()
    {
        var path = Path.Combine(Jbh.AppManager.DataPath, "Data.prevu");
        Jbh.AppManager.CreateBackupDataFile(path);
        Jbh.AppManager.PurgeOldBackups("prevu", 10, 10);
        using var writer = new StreamWriter(path);
        var dateKeys = _forecast.Keys.ToList();
        dateKeys.Sort();
        
        foreach (var key in dateKeys)
        {
            var log = _forecast[key];
            writer.WriteLine(log.Specification);
        }
    }

    private void PrevuWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        SaveData();
    }

    private static DateOnly DateToday => DateOnly.FromDateTime(DateTime.Today);

    private void RefreshCalendar()
    {
        EditDateButton.IsEnabled = false;
        DeletePastButton.IsEnabled = false;
        var wid = PrevuListBox.ActualWidth - 40;
        PurgeEmptyDays();
        var hasPast = false;

        PrevuListBox.Items.Clear();

        var busyDates = new List<int>();
        var infoDates = new List<int>();

        var doneCatchUp = false;
        var counter = -1;

        var keys = _forecast.Keys.ToList();
        keys.Sort();
        foreach (var dateKey in keys)
        {
            var jour = _forecast[dateKey];
            counter++;

            if (!jour.Empty)
            {
                if (jour.InfoOnly)
                {
                    infoDates.Add(jour.EntryDate.DayNumber);
                }
                else
                {
                    busyDates.Add(jour.EntryDate.DayNumber);
                }
            }

            var gap = jour.EntryDate.DayNumber - DateToday.DayNumber;

            if (gap < 0)
            {
                hasPast = true;
            }
            else
            {
                // Once only, after finishing with past entries, show non-selectable blue dates from today up to plannerEntry-1
                if (!doneCatchUp)
                {
                    var moment = DateToday;
                    while (moment.DayNumber < jour.EntryDate.DayNumber)
                    {
                        PrevuListBox.Items.Add(new ListBoxItem()
                        {
                            IsHitTestVisible = false, Tag = -99, Content = new Border()
                            {
                                MinWidth = 256, Background = Brushes.CornflowerBlue
                                , CornerRadius = new CornerRadius(4, 4, 4, 4), Padding = new Thickness(6, 4, 6, 4)
                                , Child = new TextBlock()
                                {
                                    Text = $"{moment:dddd dd MMM}", Foreground = Brushes.Ivory
                                    , FontFamily = new FontFamily("Liberation Mono"), FontSize = 18
                                }
                            }
                        });
                        moment = moment.AddDays(1);
                    }

                    doneCatchUp = true;
                }
            }

            // colour for past, present, future dates
            Brush pinceau = gap < 0 ? Brushes.Sienna : gap > 0 ? Brushes.Black : Brushes.Blue;

            var dateDock = new DockPanel();
            PrevuListBox.Items.Add(new ListBoxItem() {Tag = counter, Content = new Border()
            {
                CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(2)
                , Padding = new Thickness(8), Child = dateDock, Width = wid
            }});

            var whenBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Comic Sans MS"), FontSize = 16, Foreground = pinceau
                , Margin = new Thickness(0, 0, 0, 8)
                , Text = $"{jour.EntryDate:ddd dd MMM yyyy} "
            };

            var awayString = gap == 1 ? " TOMORROW" :
                gap > 0 ? $" {gap} days away" :
                gap < 0 ? $" {Math.Abs(gap)} days ago" : " TODAY";

            whenBloc.Inlines.Add(new Run() {Text = awayString, FontSize = 12, Foreground = Brushes.Magenta});
            DockPanel.SetDock(whenBloc, Dock.Top);
            dateDock.Children.Add(whenBloc);

            foreach (var action in jour.Actions)
            {
                var actionDock = new DockPanel();
                var actionBorder=new Border()
                {
                    CornerRadius = new CornerRadius(3), BorderBrush = Brushes.DarkRed, BorderThickness = new Thickness(1)
                    , Padding = new Thickness(8), Child = actionDock, Background = Brushes.Pink
                };
                
                DockPanel.SetDock(actionBorder, Dock.Top);
                dateDock.Children.Add(actionBorder);
                
                if (action.StartTime > TimeOnly.MinValue)
                {
                    var timeBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), FontSize = 16, Foreground = Brushes.DarkRed
                        , Text = $"{action.StartTime:HH.mm}"
                    };
                    if (action.EndTime > action.StartTime)
                    {
                        timeBloc.Inlines.Add(new Run() {Text = $" to {action.EndTime:HH.mm}"});
                    }

                    DockPanel.SetDock(timeBloc, Dock.Top);
                    actionDock.Children.Add(timeBloc);
                }

                var descriptionBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), FontSize = 16
                    , Foreground = jour.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
                    , Text = action.Description
                    , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 8)
                };

                DockPanel.SetDock(descriptionBloc, Dock.Top);
                actionDock.Children.Add(descriptionBloc);

                if (!string.IsNullOrWhiteSpace(action.Notes))
                {
                    var notesBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), Foreground = Brushes.DarkRed, Text = action.Notes
                        , Margin = new Thickness(0, 0, 0, 8)
                        , TextWrapping = TextWrapping.Wrap
                    };

                    DockPanel.SetDock(notesBloc, Dock.Top);
                    actionDock.Children.Add(notesBloc);
                }

                var outlookBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono")
                    , Foreground = action.AddedToOutlook ? Brushes.DarkGreen : Brushes.Red
                    , Text = action.AddedToOutlook ? "Added to Outlook" : "Not added to Outlook"
                    , TextWrapping = TextWrapping.Wrap
                };

                DockPanel.SetDock(outlookBloc, Dock.Top);
                actionDock.Children.Add(outlookBloc);
            }

            foreach (var info in jour.Infos)
            {
                var infoDock = new DockPanel();
                
                var infoBorder = new Border()
                {
                    CornerRadius = new CornerRadius(3), BorderBrush = Brushes.DarkOrange, BorderThickness = new Thickness(1)
                    , Padding = new Thickness(8),Child =infoDock, Background = Brushes.LightYellow, Margin = new Thickness(0, 4 ,0 ,4)
                };
                
                DockPanel.SetDock(infoBorder, Dock.Top);
                dateDock.Children.Add(infoBorder);
                
                var descriptionBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), FontSize = 16
                    , Foreground = Brushes.SeaGreen
                    , Text = info.Description
                    , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 8)
                };

                DockPanel.SetDock(descriptionBloc, Dock.Top);
                infoDock.Children.Add(descriptionBloc);

                if (!string.IsNullOrWhiteSpace(info.Notes))
                {
                    var notesBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), Foreground = Brushes.SeaGreen, Text = info.Notes
                        , Margin = new Thickness(0, 0, 0, 8)
                        , TextWrapping = TextWrapping.Wrap
                    };

                    DockPanel.SetDock(notesBloc, Dock.Top);
                    infoDock.Children.Add(notesBloc);
                }

            }

            // indicate occupied dates in top row
            BlobCanvas.Children.Clear();
            double xPosition = 0;
            var zero = DateToday.DayNumber;
            var a = 0;
            while (xPosition < BlobCanvas.ActualWidth)
            {
                int dayIndex = zero + a;
                Brush blobColour = busyDates.Contains(dayIndex) ? Brushes.OrangeRed :
                    infoDates.Contains(dayIndex) ? Brushes.SeaGreen : Brushes.CornflowerBlue;
                DateOnly when = DateOnly.FromDayNumber(dayIndex);
                if (Weekend(when))
                {
                    var rect = new Rectangle() {Width = 16, Height = 16, Fill = blobColour};
                    Canvas.SetLeft(rect, xPosition);
                    Canvas.SetTop(rect, 0);
                    BlobCanvas.Children.Add(rect);
                    xPosition += 18;
                }
                else
                {
                    var elly = new Ellipse() {Width = 12, Height = 12, Fill = blobColour};
                    Canvas.SetLeft(elly, xPosition);
                    Canvas.SetTop(elly, 2);
                    BlobCanvas.Children.Add(elly);
                    xPosition += 14;
                }

                a++;

            }
        }

        DeletePastButton.IsEnabled = hasPast;
    }

    private void PrevuWindow_OnContentRendered(object? sender, EventArgs e)
    {
        PopulateDateCombo();
        RefreshCalendar();
    }

    private void DeletePastAppointmentButton_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBoxResult decision = MessageBox.Show("Delete all past appointments?", "Delete entries"
            , MessageBoxButton.YesNo
            , MessageBoxImage.Question);
        if (decision == MessageBoxResult.No) return;

        // Remove all past entries and refresh list
        var target = 99;
        while (target >= 0)
        {
            target = -1;
            foreach (var key in _forecast.Keys)
            {
                if (_forecast[key].EntryDate < DateToday)
                {
                    target = key;
                }
            }

            if (target >= 0)
            {
                _forecast.Remove(target);
            }
        }
        RefreshCalendar();
    }

    private static bool Weekend(DateOnly q)
    {
        return q.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    private void EditDateButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (EditDateButton.Tag is not DateOnly when) return;
        var jour = new PrevuDate(){EntryDate = when};
        if (_forecast.ContainsKey(when.DayNumber))
        {
            jour = _forecast[when.DayNumber];
        }
        var editor = new PrevuEditor(jour){Owner = this};
        var returnValue = editor.ShowDialog();
        if (!(returnValue ?? false)) return;

        if (_forecast.ContainsKey(when.DayNumber))
        {
            _forecast[when.DayNumber].Specification = editor.PrevuDateSpecification;
        }
        else
        {
            var pd = new PrevuDate() {Specification = editor.PrevuDateSpecification};
            _forecast.Add(when.DayNumber, pd);
        }
        RefreshCalendar();
    }

    private void DateComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        EditDateButton.IsEnabled = false;
        if (DateComboBox.SelectedItem is not ComboBoxItem {Tag: DateOnly when}) return;
        EditDateButton.Tag = when;
        EditDateButton.IsEnabled = true;
    }
    
    private void PopulateDateCombo()
    {
        DateComboBox.Items.Clear();
        var dateO = DateOnly.FromDateTime(DateTime.Today);
        var todayIndex = dateO.DayNumber;
        while (dateO.DayNumber - todayIndex < 366)
        {
            bool weekend = Weekend(dateO);
            DateComboBox.Items.Add(new ComboBoxItem()
            {
                Tag = dateO
                , Content = new TextBlock()
                {
                    Text = $"{dateO:ddd dd MMM yyyy}", FontFamily = new FontFamily("Liberation Mono")
                    , Foreground = weekend ? Brushes.Blue : Brushes.Black
                }
            });
            dateO = dateO.AddDays(1);
        }
    }
    
}