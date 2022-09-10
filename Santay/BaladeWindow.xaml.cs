using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Santay;

public partial class BaladeWindow
{
    private Balade _ride;
    // private readonly DateTime _originalDate;
    private readonly DateOnly _originalDateOnly;
    private readonly List<string> _groupList;

    public BaladeWindow(Balade trip, List<string> gpList)
    {
        InitializeComponent();
        _ride = trip;
        _originalDateOnly = _ride.TripDate;
        //_originalDate = _ride.RideDate;
        _groupList = gpList;
    }

    public Balade TripDetails => _ride;

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        GroupComboBox.Items.Clear();
        foreach (string s in _groupList)
        {
            GroupComboBox.Items.Add(s);
        }

        DifficultyComboBox.Items.Clear();
        for (int d = 1; d < 4; d++)
        {
            DifficultyComboBox.Items.Add(Balade.DifficultyCaption(d));
        }

        DateInputBox.DateValue = _ride.TripDate.ToDateTime(TimeOnly.MinValue);
        DistJInputBox.Text = _ride.RideKm.ToString(CultureInfo.CurrentCulture);
        LocnInputBox.Text = _ride.RideCaption;
        GroupComboBox.Text = _ride.RideGroup;
        DifficultyComboBox.SelectedIndex = _ride.Difficulty - 1;
        
        DateInputBox.Focus();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DateTime dt;
        if (DateInputBox.DateValue.HasValue)
        {
            dt = DateInputBox.DateValue.Value.Date;
        }
        else
        {
            MessageBox.Show("You must enter a valid date", "Input error", MessageBoxButton.OK
                , MessageBoxImage.Asterisk);
            return;
        }

        string dstJ = DistJInputBox.Text;
        bool dstOkJ = double.TryParse(dstJ, out double kilomJ);
        if (!dstOkJ)
        {
            MessageBox.Show("You must enter a valid distance in Km\ne.g. 34.5", "Input error", MessageBoxButton.OK
                , MessageBoxImage.Asterisk);
            return;
        }

        string locn = LocnInputBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(locn))
        {
            MessageBox.Show("You must enter a route or location", "Input error", MessageBoxButton.OK
                , MessageBoxImage.Asterisk);
            return;
        }

        string grup = GroupComboBox.Text;
        int diffy = DifficultyComboBox.SelectedIndex + 1;

        // VeloHistory.TripType k = VeloHistory.TripType.Walk;
        // if (RideRadio.IsChecked.HasValue && RideRadio.IsChecked.Value)
        // {
        //     k = VeloHistory.TripType.Cycle;
        // }

        _ride = new Balade(dat: dt, kmJ: kilomJ, cp: locn, gp: grup, diff: diffy);
        DialogResult = true;
    }

    public bool DateAltered => !_originalDateOnly.Equals(_ride.TripDate);

    private void InputBox_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox tb = (TextBox) sender;
        tb.SelectAll();
    }

    private void BtnMinus_Click(object sender, RoutedEventArgs e)
    {
        if (DateInputBox.DateValue.HasValue)
        {
            DateTime d = DateInputBox.DateValue.Value;
            d = d.AddDays(-1);
            DateInputBox.DateValue = d;
        }
    }

    private void BtnPlus_Click(object sender, RoutedEventArgs e)
    {
        if (DateInputBox.DateValue.HasValue)
        {
            DateTime d = DateInputBox.DateValue.Value;
            d = d.AddDays(1);
            DateInputBox.DateValue = d;
        }
    }
}