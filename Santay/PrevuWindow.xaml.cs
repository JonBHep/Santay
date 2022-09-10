using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        _schedule = new List<PrevuDate>();
    }

    private readonly List<PrevuDate> _schedule;

    private void PlannerWindow_OnLoaded(object sender, RoutedEventArgs e)
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
            _schedule.Clear();
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var j = reader.ReadLine();
                if (j is null) continue;
                var log = new PrevuDate() {Specification = j};
                _schedule.Add(log);
            }
        }
    }

    private void SaveData()
    {
        var path = Path.Combine(Jbh.AppManager.DataPath, "Data.prevu");
        Jbh.AppManager.CreateBackupDataFile(path);
        Jbh.AppManager.PurgeOldBackups("prevu", 10, 10);
        using var writer = new StreamWriter(path);
        _schedule.Sort();

        foreach (var log in _schedule)
        {
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

        var hasPast = false;

        PrevuListBox.Items.Clear();

        var busyDates = new List<int>();
        var infoDates = new List<int>();

        var doneCatchUp = false;
        var counter = -1;

        foreach (var prevuDate in _schedule)
        {
            counter++;

            if (!prevuDate.Empty)
            {
                if (prevuDate.InfoOnly)
                {
                    infoDates.Add(prevuDate.EntryDate.DayNumber);
                }
                else
                {
                    busyDates.Add(prevuDate.EntryDate.DayNumber);
                }
            }

            var gap = prevuDate.EntryDate.DayNumber - DateToday.DayNumber;

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
                    while (moment.DayNumber < prevuDate.EntryDate.DayNumber)
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

            var outline = new Border()
            {
                CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(2)
                , Padding = new Thickness(8)
            };
            var dateDock = new DockPanel();
            outline.Child = dateDock;
            PrevuListBox.Items.Add(new ListBoxItem() {Tag = counter, Content = outline});

            var dateBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Comic Sans MS"), FontSize = 16, Foreground = pinceau
                , Margin = new Thickness(0, 0, 0, 8)
                , Text = $"{prevuDate.EntryDate:ddd dd MMM yyyy} "
            };

            var awayString = gap == 1 ? " TOMORROW" :
                gap > 0 ? $" {gap} days away" :
                gap < 0 ? $" {Math.Abs(gap)} days ago" : " TODAY";

            dateBloc.Inlines.Add(new Run() {Text = awayString, FontSize = 12, Foreground = Brushes.Magenta});

            DockPanel.SetDock(dateBloc, Dock.Top);

            foreach (var action in prevuDate.Actions)
            {
                var actionBorder = new Border()
                {
                    CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(1)
                    , Padding = new Thickness(8)
                };
                var actionDock = new DockPanel();
                actionBorder.Child = actionDock;
                DockPanel.SetDock(actionBorder, Dock.Top);
                dateDock.Children.Add(actionBorder);

                if (action.StartTime > TimeOnly.MinValue)
                {
                    var timeBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), FontSize = 16, Foreground = pinceau
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
                    , Foreground = prevuDate.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
                    , Text = action.Description
                    , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 8)
                };

                DockPanel.SetDock(descriptionBloc, Dock.Top);
                actionDock.Children.Add(descriptionBloc);

                if (!string.IsNullOrWhiteSpace(action.Notes))
                {
                    var notesBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), Foreground = pinceau, Text = action.Notes
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

            foreach (var info in prevuDate.Infos)
            {
                var infoBorder = new Border()
                {
                    CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(1)
                    , Padding = new Thickness(8)
                };
                var infoDock = new DockPanel();
                infoBorder.Child = infoDock;
                dateDock.Children.Add(infoBorder);
                DockPanel.SetDock(infoBorder, Dock.Top);

                var descriptionBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), FontSize = 16
                    , Foreground = prevuDate.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
                    , Text = info.Description
                    , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 8)
                };

                DockPanel.SetDock(descriptionBloc, Dock.Top);
                dateDock.Children.Add(descriptionBloc);

                if (!string.IsNullOrWhiteSpace(info.Notes))
                {
                    var notesBloc = new TextBlock()
                    {
                        FontFamily = new FontFamily("Liberation Mono"), Foreground = pinceau, Text = info.Notes
                        , Margin = new Thickness(0, 0, 0, 8)
                        , TextWrapping = TextWrapping.Wrap
                    };

                    DockPanel.SetDock(notesBloc, Dock.Top);
                    dateDock.Children.Add(notesBloc);
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

    private void PlannerWindow_OnContentRendered(object? sender, EventArgs e)
    {
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
            for (int index = 0; index < _schedule.Count; index++)
            {
                if (_schedule[index].EntryDate < DateToday)
                {
                    target = index;
                }
            }

            if (target >= 0)
            {
                _schedule.RemoveAt(target);
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
        throw new NotImplementedException();
    }

    private void NewDateButton_OnClick(object sender, RoutedEventArgs e)
    {
        var edwin = new PrevuEditor() {Owner = this};
        _ = edwin.ShowDialog();
    }
    
}