using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Santay;

public partial class ListBloodPressureWindow
{
    public ListBloodPressureWindow()
    {
        InitializeComponent();
        _coreData = new PersonProfile();
    }

    private readonly PersonProfile _coreData;
    
    private void RefreshReadings()
    {
        TensionListBox.Items.Clear();
        _coreData.TensionReadings.Sort();
        foreach (var p in _coreData.TensionReadings)
        {
            var hPanel = new StackPanel() {Orientation = Orientation.Horizontal};
            hPanel.Children.Add(new TextBlock() {Text = p.BprWhen.ToShortDateString(), Width = 128});
            string timeString = p.BprWhen.ToShortTimeString();
            if (timeString=="00:00"){timeString=string.Empty;}
            hPanel.Children.Add(new TextBlock() {Text = timeString, Width = 128});
            hPanel.Children.Add(new TextBlock() {Text = $"{p.BpSystolic} / {p.BpDiastolic}", Width = 128});
            if (p.Pulse > 0)
            {
                hPanel.Children.Add(new TextBlock() {Text = $"Pulse {p.Pulse}", Width = 128});    
            }
            TensionListBox.Items.Add(hPanel);
        }

        TensionListBox.SelectedIndex = TensionListBox.Items.Count - 1;
        TensionListBox.ScrollIntoView(TensionListBox.SelectedItem);
    }

    private void ListviewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TensionListBox.SelectedItem == null)
        {
            ButtonDelete.IsEnabled = false;
            ButtonEdit.IsEnabled = false;
        }
        else
        {
            ButtonDelete.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
        }
    }

    private void ButtonCloseClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ButtonAdd_Click(object sender, RoutedEventArgs e)
    {
        EnterBloodPressureWindow w = new EnterBloodPressureWindow(){Owner = this};
        w.PopulateWith(DateTime.Today, 0, 0, 0);
        if (w.ShowDialog() == true)
        {
            _coreData.AddBloodPressureReading(w.ReadingDate, w.RvDiastolic, w.RvSystolic, w.RvPulse);
            RefreshReadings();
        }
    }

    private void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        int y = TensionListBox.SelectedIndex;
        PersonProfile.Tension r = _coreData.TensionReadings[y];
        EnterBloodPressureWindow w = new EnterBloodPressureWindow(){Owner = this};
        w.PopulateWith(r.BprWhen, r.BpDiastolic, r.Pulse, r.BpSystolic);
        if (w.ShowDialog() ?? false)
        {
            _coreData.EditBloodPressureReading(y, w.ReadingDate, w.RvDiastolic, w.RvSystolic, w.RvPulse);
            RefreshReadings();
        }
    }

    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        int y = TensionListBox.SelectedIndex;
        PersonProfile.Tension r = _coreData.TensionReadings[y];
        string message
            = $"Blood pressure reading\n\nDate: {r.BprWhen.ToLongDateString()}\n{r.BpSystolic}/{r.BpDiastolic} and {r.Pulse}";
        if (MessageBox.Show(message, "Delete reading?", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
            MessageBoxResult.No)
        {
            return;
        }

        _coreData.TensionReadings.RemoveAt(y);
        RefreshReadings();
    }

    private void ListBloodPressureWindow_OnContentRendered(object? sender, EventArgs e)
    {
        RefreshReadings();
    }

    private void ListBloodPressureWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        _coreData.SaveAllData();
    }
    
}