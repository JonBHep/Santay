using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Santay;

public partial class PrevuEditor
{
    public PrevuEditor(PrevuDate jour)
    {
        InitializeComponent();
        _journee = new PrevuDate(jour);
    }
    public PrevuEditor()
    {
        InitializeComponent();
        _journee = new PrevuDate();
    }
    
    private readonly PrevuDate _journee;

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
        ActionEditorBorder.Visibility = InfoEditorBorder.Visibility = Visibility.Collapsed;
    }

    private static DateOnly DateToday => DateOnly.FromDateTime(DateTime.Today);

    private void RefreshDayList()
    {
        EditActionButton.IsEnabled = false;
        DeleteActionButton.IsEnabled = false;
        EditInfoButton.IsEnabled = false;
        DeleteInfoButton.IsEnabled = false;

        DayListBox.Items.Clear();

        var gap = _journee.EntryDate.DayNumber - DateToday.DayNumber;

        Brush pinceau = gap < 0 ? Brushes.Sienna : gap > 0 ? Brushes.Black : Brushes.Blue;

        var outline = new Border()
        {
            CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(2)
            , Padding = new Thickness(8)
        };
        var dateDock = new DockPanel();
        outline.Child = dateDock;
        DayListBox.Items.Add(new ListBoxItem() {Content = outline});

        var dateBloc = new TextBlock()
        {
            FontFamily = new FontFamily("Comic Sans MS"), FontSize = 16, Foreground = pinceau
            , Margin = new Thickness(0, 0, 0, 8)
            , Text = $"{_journee.EntryDate:ddd dd MMM yyyy} "
        };

        var awayString = gap == 1 ? " TOMORROW" :
            gap > 0 ? $" {gap} days away" :
            gap < 0 ? $" {Math.Abs(gap)} days ago" : " TODAY";

        dateBloc.Inlines.Add(new Run() {Text = awayString, FontSize = 12, Foreground = Brushes.Magenta});

        DockPanel.SetDock(dateBloc, Dock.Top);

        foreach (var action in _journee.Actions)
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
                , Foreground = _journee.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
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

        foreach (var info in _journee.Infos)
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
                , Foreground = _journee.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
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
    }



private void PrevuWindow_OnContentRendered(object? sender, EventArgs e)
    {
        RefreshDayList();
        PopulateDateAndTimesCombos();
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
        return entry;
    }

    private static bool Weekend(DateOnly q)
    {
        return q.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    private void NewActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Visible;
        InfoEditorBorder.Visibility = Visibility.Collapsed;
    }

    private void NewInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Collapsed;
        InfoEditorBorder.Visibility = Visibility.Visible;
    }

    private void SwitchButtonPanels(bool live)
    {
        InfosPanel.Opacity =ActionsPanel.Opacity= live ? 1 : 0.5;
        InfosPanel.IsEnabled = ActionsPanel.IsEnabled = live;
    }

    private void ActionEditorCancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        ActionEditorBorder.Visibility = Visibility.Collapsed;
        SwitchButtonPanels(true);
    }
    
    private void InfoEditorCancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        InfoEditorBorder.Visibility = Visibility.Collapsed;
        SwitchButtonPanels(true);
    }
}