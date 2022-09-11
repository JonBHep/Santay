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
    
    private readonly PrevuDate _journee;
    private const int IndexOffset = 65536; // (2 to the power of 16)
    
    public string PrevuDateSpecification => _journee.Specification;
    

    private void PrevuEditor_OnLoaded(object sender, RoutedEventArgs e)
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
        
        _journee.Actions.Sort();
        _journee.Infos.Sort();
        
        // List Box Item tags
        // ListBoxItem for date heading is Hit Test Invisible
        // ListBoxItem for Action has int = index
        // ListBoxItem for Info has int IndexOffset + index
        // index is negative for new item
        
        var gap = _journee.EntryDate.DayNumber - DateToday.DayNumber;

        Brush pinceau = gap < 0 ? Brushes.Sienna : gap > 0 ? Brushes.Black : Brushes.Blue;

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
        
        // add item showing date to listbox
        DayListBox.Items.Add(new ListBoxItem() {Tag ="D",IsHitTestVisible =false, Content = new Border()
        {
            CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(2)
            , Padding = new Thickness(8), Child = dateBloc
        }});
        
        for (var z=0; z< _journee.Actions.Count; z++)
        {
            var action = _journee.Actions[z];
            
            var actionDock = new DockPanel();
            
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
            
            var actionBorder = new Border()
            {
                CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(1)
                , Padding = new Thickness(8), Child = actionDock
            };
            
            DayListBox.Items.Add(new ListBoxItem() {Tag =z, Content = actionBorder});
        }

        for (var z=0; z< _journee.Infos.Count; z++)
        {
            var info = _journee.Infos[z];
            
            var infoDock = new DockPanel();
            
            var descriptionBloc = new TextBlock()
            {
                FontFamily = new FontFamily("Liberation Mono"), FontSize = 16
                , Foreground = _journee.InfoOnly ? Brushes.SeaGreen : Brushes.OrangeRed
                , Text = info.Description
                , TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 8)
            };

            DockPanel.SetDock(descriptionBloc, Dock.Top);
            infoDock.Children.Add(descriptionBloc);

            if (!string.IsNullOrWhiteSpace(info.Notes))
            {
                var notesBloc = new TextBlock()
                {
                    FontFamily = new FontFamily("Liberation Mono"), Foreground = pinceau, Text = info.Notes
                    , Margin = new Thickness(0, 0, 0, 8)
                    , TextWrapping = TextWrapping.Wrap
                };

                DockPanel.SetDock(notesBloc, Dock.Top);
                infoDock.Children.Add(notesBloc);
            }
            
            var infoBorder = new Border()
            {
                CornerRadius = new CornerRadius(3), BorderBrush = pinceau, BorderThickness = new Thickness(1)
                , Padding = new Thickness(8), Child = infoDock
            };
            
            DayListBox.Items.Add(new ListBoxItem() {Tag=z+IndexOffset, Content = infoBorder});
        }

    }

    private void PrevuEditor_OnContentRendered(object? sender, EventArgs e)
    {
        PopulateTimeCombos();
        RefreshDayList();
    }

    private void FillActionEntryForm(PrevuAction acta)
    {
        ComboBoxItem? target = null;
        
        foreach (var item in StartHourComboBox.Items)
        {
            if (item is ComboBoxItem {Tag: int shr} cItem)
            {
                if (shr.Equals(acta.StartTime.Hour))
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
                if (smn.Equals(acta.StartTime.Minute))
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
                if (ehr.Equals(acta.EndTime.Hour))
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
                if (emn.Equals(acta.EndTime.Minute))
                {
                    target = cItem;
                }
            }
        }

        EndMinuteComboBox.SelectedItem = target;

        DescriptionTextBox.Text = acta.Description;

        NotesTextBox.Text = acta.Notes;

        OutlookCheckBox.IsChecked = acta.AddedToOutlook;

    }
    
    private void FillInfoEntryForm(PrevuInfo info)
    {
        InfoDescriptionTextBox.Text = info.Description;

        InfoNotesTextBox.Text = info.Notes;
    }
   
    private void PopulateTimeCombos()
    {
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

    private PrevuAction? HarvestPrevuAction()
    {
        var entry = new PrevuAction();
    
        if (StartHourComboBox.SelectedItem is ComboBoxItem {Tag: int startH})
        {
            entry.StartTime = StartMinuteComboBox.SelectedItem is ComboBoxItem {Tag: int startM} ? new TimeOnly(startH, startM) : new TimeOnly(startH, 0);
        }
        else
        {
            entry.StartTime = new TimeOnly(0, 0);
        }

        if (EndHourComboBox.SelectedItem is ComboBoxItem {Tag: int endH})
        {
            entry.EndTime = EndMinuteComboBox.SelectedItem is ComboBoxItem {Tag: int endM} ? new TimeOnly(endH, endM) : new TimeOnly(endH, 0);
        }
        else
        {
            entry.EndTime = new TimeOnly(0, 0);
        }

        var caption = DescriptionTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(caption))
        {
            return null;
        }

        entry.Description = caption;

        var comments = NotesTextBox.Text.Trim();
        entry.Notes = string.IsNullOrWhiteSpace(comments) ? string.Empty : comments;

        entry.AddedToOutlook = OutlookCheckBox.IsChecked ?? false;
        return entry;
    }
    
    private PrevuInfo? HarvestPrevuInfo()
    {
        var entry = new PrevuInfo();
    
        var caption = InfoDescriptionTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(caption))
        {
            return null;
        }

        entry.Description = caption;

        var comments = InfoNotesTextBox.Text.Trim();
        entry.Notes = string.IsNullOrWhiteSpace(comments) ? string.Empty : comments;

        return entry;
    }
    
    private void NewActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Visible;
        InfoEditorBorder.Visibility = Visibility.Collapsed;
        ClearActionEntryForm();
        ApplyActionButton.Tag = -1;
    }

    private void ClearActionEntryForm()
    {
        StartHourComboBox.SelectedIndex = 0;
        StartMinuteComboBox.SelectedIndex = 0;
        EndHourComboBox.SelectedIndex = 0;
        EndMinuteComboBox.SelectedIndex = 0;
        DescriptionTextBox.Clear();
        NotesTextBox.Clear();
        OutlookCheckBox.IsChecked = false;
    }
    private void ClearInfoEntryForm()
    {
        InfoDescriptionTextBox.Clear();
        InfoNotesTextBox.Clear();
    }

    private void NewInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Collapsed;
        InfoEditorBorder.Visibility = Visibility.Visible;
        ClearInfoEntryForm();
        ApplyInfoButton.Tag = -1;
    }

    private void SwitchButtonPanels(bool live)
    {
        InfosPanel.Opacity = ActionsPanel.Opacity = live ? 1 : 0.5;
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

    private void ApplyActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ApplyActionButton.Tag is not int index) return;
        var gathered = HarvestPrevuAction();
        if (gathered is {})
        {
            if (index >= 0)
            {
                _journee.Actions.RemoveAt(index);
            }
            _journee.Actions.Add(gathered);
            _journee.Actions.Sort();
            ActionEditorBorder.Visibility = Visibility.Collapsed;
            SwitchButtonPanels(true);
            RefreshDayList();
        }
        else
        {
            MessageBox.Show("Incomplete", "Action", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }

    private void ApplyInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (ApplyInfoButton.Tag is not int index) return;
        var gathered = HarvestPrevuInfo();
        if (gathered is { })
        {
            if (index >= 0)
            {
                _journee.Infos.RemoveAt(index);
            }
            _journee.Infos.Add(gathered);
            _journee.Actions.Sort();
            InfoEditorBorder.Visibility = Visibility.Collapsed;
            SwitchButtonPanels(true);
            RefreshDayList();
        }
        else
        {
            MessageBox.Show("Incomplete", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
   
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void DayListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DayListBox.SelectedItem is not (ListBoxItem {Tag: int tag})) return;
        if (tag >= IndexOffset)
        {
            var index = tag-IndexOffset;
            EditActionButton.IsEnabled = DeleteActionButton.IsEnabled =false;
            EditInfoButton.Tag = DeleteInfoButton.Tag = index;
            EditInfoButton.IsEnabled = DeleteInfoButton.IsEnabled = true;
        }
        else
        {
            EditActionButton.IsEnabled = DeleteActionButton.IsEnabled = true;
            EditActionButton.Tag = DeleteActionButton.Tag = tag;
            EditInfoButton.IsEnabled = DeleteInfoButton.IsEnabled = false;
        }
    }

    private void EditActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (EditActionButton.Tag is not int index) return;
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Visible;
        InfoEditorBorder.Visibility = Visibility.Collapsed;
        ClearActionEntryForm();
        FillActionEntryForm(_journee.Actions[index]);
        ApplyActionButton.Tag = index;
    }
    private void EditInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (EditInfoButton.Tag is not int index) return;
        SwitchButtonPanels(false);
        ActionEditorBorder.Visibility = Visibility.Collapsed;
        InfoEditorBorder.Visibility = Visibility.Visible;
        ClearInfoEntryForm();
        FillInfoEntryForm(_journee.Infos[index]);
        ApplyInfoButton.Tag = index;
    }

    private void DeleteActionButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DeleteActionButton.Tag is not int index) return;
        _journee.Actions.RemoveAt(index);
        RefreshDayList();
    }
    private void DeleteInfoButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (DeleteInfoButton.Tag is not int index) return;
        _journee.Infos.RemoveAt(index);
        RefreshDayList();
    }
    
}