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

public partial class PlannerWindow
{

    private readonly List<PlannerEntry> _schedule;

    public PlannerWindow()
    {
        InitializeComponent();
        _schedule = new List<PlannerEntry>();
    }

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
        var path = Path.Combine(Jbh.AppManager.DataPath, "Data.planner");
        if (File.Exists(path))
        {
            _schedule.Clear();
            using var reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                var j = reader.ReadLine();
                if (j is null) continue;
                var log = new PlannerEntry() {Specification = j};
                _schedule.Add(log);
            }
        }
    }

    private void SaveData()
    {
        var path = Path.Combine(Jbh.AppManager.DataPath, "Data.planner");
        Jbh.AppManager.CreateBackupDataFile(path);
        Jbh.AppManager.PurgeOldBackups("planner", 10, 10);
        using var writer = new StreamWriter(path);
        _schedule.Sort();

        foreach (var log in _schedule)
        {
            writer.WriteLine(log.Specification);
        }
    }

    private void PlannerWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        SaveData();
    }

    private static DateOnly DateToday => DateOnly.FromDateTime(DateTime.Today);

    private void RefreshAppointmentList()
    {
        var wid = PlannerListBox.ActualWidth - 32;
        BlobCanvas.Width = wid;
        
        EditAppointmentButton.IsEnabled = false;
        DeleteAppointmentButton.IsEnabled = false;
        DeletePastButton.IsEnabled = false;

        var hasPast = false;

        EntryEditorBorder.Visibility = Visibility.Hidden;
        PlannerListBox.Items.Clear();

        // var occupiedDates = _schedule.Select(plannerEntry => plannerEntry.EntryDate.DayNumber).ToList(); // LINQ expression
        var busyDates = new List<int>();
        var infoDates = new List<int>();
        
        var doneCatchUp = false;
        var counter = -1;
        
        foreach (var plannerEntry in _schedule)
        {
            counter++;
            
            if (plannerEntry.InfoOnly){infoDates.Add(plannerEntry.EntryDate.DayNumber);} else {busyDates.Add(plannerEntry.EntryDate.DayNumber);}
            
            var gap = plannerEntry.EntryDate.DayNumber - DateToday.DayNumber;
            
            if (gap < 0)
            {
                hasPast = true;
            }
            else
            {
                // Once only, after finishing with past entries, add non-selectable blue dates from today up to plannerEntry-1
                if (!doneCatchUp)
                {
                    var moment = DateToday;
                    while (moment.DayNumber < plannerEntry.EntryDate.DayNumber)
                    {
                        PlannerListBox.Items.Add(new ListBoxItem()
                        {
                            IsHitTestVisible = false,Tag=-99, Content = new Border(){MinWidth = 256, Background =Brushes.CornflowerBlue,CornerRadius =new CornerRadius(4,4,4,4), Padding =new Thickness(6,4,6,4), Child =  new TextBlock()
                            {
                                Text = $"{moment:dddd dd MMM}", Foreground = Brushes.Ivory
                                , FontFamily = new FontFamily("Liberation Mono"), FontSize=18
                            }
                        }});
                        moment = moment.AddDays(1);
                    }
                    doneCatchUp = true;
                }
            }
            
            Brush pinceau = gap < 0 ? Brushes.Sienna : gap > 0 ? Brushes.Black : Brushes.Blue;
            var outline = new Border() {CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(2), Width = wid, Padding = new Thickness(8)} ;
            var dock = new DockPanel();
            outline.Child = dock;
            PlannerListBox.Items.Add(new ListBoxItem() {Tag=counter, Content = outline});

            var dateBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Comic Sans MS"), FontSize = 16, Foreground = pinceau, Margin = new Thickness(0, 0, 0,8)
                , Text = $"{plannerEntry.EntryDate:ddd dd MMM yyyy} "
            };

            var awayString = gap==1 ? " TOMORROW" : gap > 0 ? $" {gap} days away" : gap < 0 ? $" {Math.Abs(gap)} days ago" : " TODAY";

            dateBloc.Inlines.Add(new Run() {Text = awayString, FontSize = 12, Foreground = Brushes.Magenta});

            DockPanel.SetDock(dateBloc, Dock.Top);
            dock.Children.Add(dateBloc);

            if (plannerEntry.StartTime > TimeOnly.MinValue)
            {
                var timeBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), FontSize = 16, Foreground = pinceau
                    , Text = $"{plannerEntry.StartTime:HH.mm}"
                };
                if (plannerEntry.EndTime > plannerEntry.StartTime)
                {
                    timeBloc.Inlines.Add(new Run() {Text = $" to {plannerEntry.EndTime:HH.mm}"});
                }

                DockPanel.SetDock(timeBloc, Dock.Top);
                dock.Children.Add(timeBloc);
            }
            
            var descriptionBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Liberation Mono"), FontSize = 16, Foreground =plannerEntry.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
                , Text = plannerEntry.Description
                , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0,8)
            };

            DockPanel.SetDock(descriptionBloc, Dock.Top);
            dock.Children.Add(descriptionBloc);

            if (!string.IsNullOrWhiteSpace(plannerEntry.Notes))
            {
                var notesBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), Foreground = pinceau, Text = plannerEntry.Notes, Margin = new Thickness(0, 0, 0,8)
                    , TextWrapping = TextWrapping.Wrap
                };

                DockPanel.SetDock(notesBloc, Dock.Top);
                dock.Children.Add(notesBloc);
            }
            
            var outlookBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Liberation Mono"), Foreground = plannerEntry.AddedToOutlook ? Brushes.DarkGreen : Brushes.Red, Text =plannerEntry.AddedToOutlook ? "Added to Outlook" : "Not added to Outlook"
                , TextWrapping = TextWrapping.Wrap
            };

            DockPanel.SetDock(outlookBloc, Dock.Top);
            dock.Children.Add(outlookBloc);

            // indicate occupied dates in top row
            BlobCanvas.Children.Clear();
            double xpos = 0;
            int zero = DateToday.DayNumber;
            int a = 0;
            while (xpos< wid)
            {
                int dayIndex = zero + a;
                Brush blobColour =busyDates.Contains(dayIndex) ? Brushes.OrangeRed : infoDates.Contains(dayIndex) ? Brushes.SeaGreen : Brushes.CornflowerBlue;
                DateOnly when = DateOnly.FromDayNumber(dayIndex);
                if (Weekend(when))
                {
                    var rect = new Rectangle() {Width = 16, Height = 16, Fill = blobColour};
                    Canvas.SetLeft(rect, xpos);
                    Canvas.SetTop(rect, 0);
                    BlobCanvas.Children.Add(rect);
                    xpos += 18;    
                }
                else
                {
                    var elly = new Ellipse() {Width = 12, Height = 12, Fill = blobColour};
                    Canvas.SetLeft(elly, xpos);
                    Canvas.SetTop(elly, 2);
                    BlobCanvas.Children.Add(elly);
                    xpos += 14;
                }
                
                a++;
                 
            }
        }

        DeletePastButton.IsEnabled = hasPast;
    }

    private void PlannerWindow_OnContentRendered(object? sender, EventArgs e)
    {
        RefreshAppointmentList();
        PopulateDateAndTimesCombos();
    }

    private void NewAppointmentButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Clear entry form and make visible, with add button labelled Add
        EntryEditorBorder.Visibility = Visibility.Visible;
        ClearEntryForm();
        AddButton.Content = "Add";
        AddButton.Tag = -1;
    }

    private void ClearEntryForm()
    {
        DateComboBox.SelectedIndex = 0;
        StartHourComboBox.SelectedIndex = 0;
        StartMinuteComboBox.SelectedIndex = 0;
        EndHourComboBox.SelectedIndex = 0;
        EndMinuteComboBox.SelectedIndex = 0;
        DescriptionTextBox.Clear();
        NotesTextBox.Clear();
        OutlookCheckBox.IsChecked = false;
        InfoCheckBox.IsChecked = false;
    }

    private void FillEntryForm(PlannerEntry entry)
    {
        ComboBoxItem? target = null;
        foreach (var item in DateComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: DateOnly d} cItem)
            {
                if (d.Equals(entry.EntryDate))
                {
                    target = cItem;
                }
            }
        }

        DateComboBox.SelectedItem = target;

        target = null;
        foreach (var item in StartHourComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: int shr} cItem)
            {
                if (shr.Equals(entry.StartTime.Hour))
                {
                    target = cItem;
                }
            }
        }

        StartHourComboBox.SelectedItem = target;

        target = null;
        foreach (var item in StartMinuteComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: int smn} cItem)
            {
                if (smn.Equals(entry.StartTime.Minute))
                {
                    target = cItem;
                }
            }
        }

        StartMinuteComboBox.SelectedItem = target;

        target = null;
        foreach (var item in EndHourComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: int ehr} cItem)
            {
                if (ehr.Equals(entry.EndTime.Hour))
                {
                    target = cItem;
                }
            }
        }

        EndHourComboBox.SelectedItem = target;

        target = null;
        foreach (var item in EndMinuteComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: int emn} cItem)
            {
                if (emn.Equals(entry.EndTime.Minute))
                {
                    target = cItem;
                }
            }
        }

        EndMinuteComboBox.SelectedItem = target;

        DescriptionTextBox.Text = entry.Description;

        NotesTextBox.Text = entry.Notes;

        OutlookCheckBox.IsChecked = entry.AddedToOutlook;

        InfoCheckBox.IsChecked = entry.InfoOnly;
    }

    private void EditAppointmentButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Populate entry form with selected entry and make visible, with add button labelled Update
        int index = PlannerSelectedIndex();
        if (index<0){return;}
        EntryEditorBorder.Visibility = Visibility.Visible;
        AddButton.Content = "Update";
        AddButton.Tag = index;
        FillEntryForm(_schedule[index]);
    }

    private void DeleteAppointmentButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Remove selected entry and refresh list
        int index = PlannerSelectedIndex();
        if (index<0){return;}
        var message
            = $"Delete the entry for {_schedule[index].EntryDate} at {_schedule[index].StartTime}?\n\n{_schedule[index].Description}";
        var decision = MessageBox.Show(message, "Delete entry", MessageBoxButton.YesNo
            , MessageBoxImage.Question);
        if (decision == MessageBoxResult.No) return;
        _schedule.RemoveAt(index);
        RefreshAppointmentList();
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

        RefreshAppointmentList();
    }

    private void PlannerListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Enable or disable the relevant buttons for edit/delete selected planner entry

        // Only enable editing if the selected item is not in the past

        var editable = false;
        int index = PlannerSelectedIndex();
        if ( index >= 0)
        {
            editable = !(_schedule[index].EntryDate < DateToday);
        }

        EditAppointmentButton.IsEnabled = DeleteAppointmentButton.IsEnabled = editable;
    }

    private int PlannerSelectedIndex()
    {
        int i = -1;
        if (PlannerListBox.SelectedItem is ListBoxItem {Tag: int index})
        {
            i = index;
        }

        return i;
    }
    
    private void PopulateDateAndTimesCombos()
    {
        DateComboBox.Items.Clear();
        DateOnly dateO = DateOnly.FromDateTime(DateTime.Today);
        int todayIndex = dateO.DayNumber;
        while ((dateO.DayNumber - todayIndex) < 366)
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

        StartHourComboBox.Items.Clear();
        EndHourComboBox.Items.Clear();
        for (int hr = 0; hr < 24; hr++)
        {
            StartHourComboBox.Items.Add(new ComboBoxItem()
            {
                Tag = hr, Content = new TextBlock() {Text = $"{hr:00}", FontFamily = new FontFamily("Liberation Mono")}
            });
            EndHourComboBox.Items.Add(new ComboBoxItem()
            {
                Tag = hr, Content = new TextBlock() {Text = $"{hr:00}", FontFamily = new FontFamily("Liberation Mono")}
            });
        }

        StartHourComboBox.SelectedIndex = 0;
        EndHourComboBox.SelectedIndex = 0;

        StartMinuteComboBox.Items.Clear();
        EndMinuteComboBox.Items.Clear();

        for (int mn = 0; mn < 60; mn += 5)
        {
            StartMinuteComboBox.Items.Add(new ComboBoxItem()
            {
                Tag = mn, Content = new TextBlock() {Text = $"{mn:00}", FontFamily = new FontFamily("Liberation Mono")}
            });
            EndMinuteComboBox.Items.Add(new ComboBoxItem()
            {
                Tag = mn, Content = new TextBlock() {Text = $"{mn:00}", FontFamily = new FontFamily("Liberation Mono")}
            });
        }

        StartMinuteComboBox.SelectedIndex = 0;
        EndMinuteComboBox.SelectedIndex = 0;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        EntryEditorBorder.Visibility = Visibility.Hidden;
    }

    private PlannerEntry? HarvestPlannerEntry()
    {
        var entry = new PlannerEntry();

        if (DateComboBox.SelectedItem is ComboBoxItem {Tag: DateOnly when})
        {
            entry.EntryDate = when;
        }
        else
        {
            return null;
        }

        if (StartHourComboBox.SelectedItem is ComboBoxItem {Tag: int startH})
        {
            if (StartMinuteComboBox.SelectedItem is ComboBoxItem {Tag: int startM})
            {
                entry.StartTime = new TimeOnly(startH, startM);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }

        if (EndHourComboBox.SelectedItem is ComboBoxItem {Tag: int endH})
        {
            if (EndMinuteComboBox.SelectedItem is ComboBoxItem {Tag: int endM})
            {
                entry.EndTime = new TimeOnly(endH, endM);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }

        var caption = DescriptionTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(caption))
        {
            return null;
        }

        entry.Description = caption;

        var comments = NotesTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(comments))
        {
            entry.Notes = string.Empty;
        }
        else
        {
            entry.Notes = comments;
        }

        entry.AddedToOutlook = OutlookCheckBox.IsChecked ?? false;
        entry.InfoOnly = InfoCheckBox.IsChecked ?? false;
        return entry;
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var entered = HarvestPlannerEntry();
        if (entered is null)
        {
            MessageBox.Show("Entry not fully specified", "Planner entry", MessageBoxButton.OK
                , MessageBoxImage.Information);
            return;
        }

        if (AddButton.Tag is int quelle)
        {
            if (quelle < 0)
            {
                _schedule.Add(entered);
            }
            else
            {
                _schedule[quelle].Specification = entered.Specification;
            }
        }

        _schedule.Sort();
        RefreshAppointmentList();
    }

    private static bool Weekend(DateOnly q)
    {
        return q.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
    
}