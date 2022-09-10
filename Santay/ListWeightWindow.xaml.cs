using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Santay;

public partial class ListWeightWindow
{
    public ListWeightWindow()
    {
        InitializeComponent();
        _coreData = new PersonProfile();
        _customDataItems = new System.Collections.ObjectModel.ObservableCollection<CustomData>();
        ListviewData.ItemsSource = _customDataItems;
        RefreshReadings();
    }

    private readonly PersonProfile _coreData;

    private struct CustomData
    {
        public string Date { get; set; }
        public string Weight { get; set; }
        public string Pounds { get; set; }
        public string Kilograms { get; set; }
        public string Bmi { get; set; }
    }

    private readonly System.Collections.ObjectModel.ObservableCollection<CustomData> _customDataItems;

    private void RefreshReadings()
    {
        _customDataItems.Clear();
        _coreData.WeightReadings.Sort();
        foreach (var p in _coreData.WeightReadings)
        {
            CustomData lvd = new CustomData
            {
                Date = p.WgtWhen.ToShortDateString(), Weight = BodyStatics.WeightAsStonesAndPoundsString(p.WgtKilograms)
                , Pounds = BodyStatics.WeightAsPounds(p.WgtKilograms).ToString("0.0")
                , Kilograms = p.WgtKilograms.ToString("0.00")
                , Bmi = BodyStatics.BmiOf(p.WgtKilograms, _coreData.HeightInMetres).ToString("0.0")
            };
            _customDataItems.Add(lvd);
        }

        ListviewData.SelectedIndex = ListviewData.Items.Count - 1;
        ListviewData.ScrollIntoView(ListviewData.SelectedItem);

        HeightTextBlock.Text = $"{_coreData.HeightInMetres:0.00} metres";
    }

    private void ListviewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ListviewData.SelectedItem == null)
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

    private void ButtonAddClick(object sender, RoutedEventArgs e)
    {
        EnterWeightWindow w = new EnterWeightWindow(_coreData.HeightInMetres) {Owner = this};
        w.PopulateWith(DateOnly.FromDateTime(DateTime.Today), 0);
        if (w.ShowDialog() == true)
        {
            _coreData.AddWeightReading(w.RvDate, w.RvKilo);
            RefreshReadings();
        }
    }

    private void buttonEdit_Click(object sender, RoutedEventArgs e)
    {
        int y = ListviewData.SelectedIndex;
        PersonProfile.Weight r = _coreData.WeightReadings[y];
        EnterWeightWindow w = new EnterWeightWindow(_coreData.HeightInMetres) {Owner = this};
        w.PopulateWith(r.WgtWhen, r.WgtKilograms);
        if (w.ShowDialog() == true)
        {
            _coreData.EditWeightReading(y, w.RvDate, w.RvKilo);
            RefreshReadings();
        }
    }

    private void buttonDelete_Click(object sender, RoutedEventArgs e)
    {
        int y = ListviewData.SelectedIndex;
        PersonProfile.Weight r = _coreData.WeightReadings[y];
        if (MessageBox.Show(
                "Weight reading\n\nDate: " + r.WgtWhen.ToLongDateString() + "\n" +
                r.WgtKilograms.ToString(CultureInfo.CurrentCulture) + " Kg", "Delete reading?", MessageBoxButton.YesNo
                , MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        _coreData.WeightReadings.RemoveAt(y);
        RefreshReadings();
    }

    private void ListWeightWindow_OnClosing(object? sender, CancelEventArgs e)
    {
        _coreData.SaveAllData();
    }
    
    private void HeightJButton_Click(object sender, RoutedEventArgs e)
    {
        InputBox box = new InputBox("My height", "Height in metres", _coreData.HeightInMetres.ToString(CultureInfo.CurrentCulture)) {Owner = this};
        bool? q = box.ShowDialog();
        if (q.HasValue && q.Value)
        {
            string ht = box.ResponseText;
            if (double.TryParse(ht, out double result))
            {
                _coreData.HeightInMetres = result;
                HeightTextBlock.Text = $"{_coreData.HeightInMetres:0.00} metres";
            }
            else
            {
                MessageBox.Show("An invalid value was entered", Jbh.AppManager.AppName, MessageBoxButton.OK
                    , MessageBoxImage.Asterisk);
            }
        }
    }
}