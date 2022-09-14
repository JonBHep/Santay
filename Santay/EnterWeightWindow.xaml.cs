using System;
using System.Windows;
using System.Windows.Controls;

namespace Santay;

public partial class EnterWeightWindow
{
    private DateOnly _inputDate;
    private double _inputKg;
    private double _inpKgrmKgm;
    private readonly double _height;
// TODO Get rid of DatePicker
    public EnterWeightWindow(double metresTall)
    {
        InitializeComponent();
        _height = metresTall;
    }

    public DateOnly RvDate=> _inputDate;
    
    public double RvKilo => _inputKg;

    private void datepickerDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!DatepickerDate.SelectedDate.HasValue)
        {
            return;
        }

        DateTime dt= DatepickerDate.SelectedDate?? new DateTime(1954,1,3);
        _inputDate =DateOnly.FromDateTime(dt);
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        TextblockResultDate.Text = string.Empty;
        TextblockResultKilos.Text = string.Empty;
        TextblockResultPounds.Text = string.Empty;
        TextblockResultStLb.Text = string.Empty;
        InpKgTextBox.Focus();
    }

    private void DisplayValues()
    {
        TextblockResultDate.Text = _inputDate.ToLongDateString();
        TextblockResultKilos.Text = _inputKg.ToString("0.0 Kg");
        TextblockResultPounds.Text = BodyStatics.WeightAsPounds(_inputKg).ToString("0.0 lb");
        TextblockResultStLb.Text = BodyStatics.WeightAsStonesAndPoundsString(_inputKg);
        TextblockResultBmi.Text = BodyStatics.BmiOf(_inputKg, _height).ToString("0.0");
    }

    public void PopulateWith(DateOnly d, double k)
    {
        {
            _inputDate = d;
            _inputKg = k;
            DatepickerDate.SelectedDate =new DateTime(_inputDate.Year, _inputDate.Month, _inputDate.Day);
            InpKgTextBox.Text = _inputKg==0 ? string.Empty: $"{_inputKg}";
            DisplayValues();
            ButtonSave.IsEnabled = false;
        }
    }

    private bool CheckKiloInput()
    {
        bool okInput = true;
        string kgString = InpKgTextBox.Text;
        if (string.IsNullOrWhiteSpace(kgString))
        {
            okInput = false;
        }

        if (double.TryParse(kgString, out double v) == false)
        {
            okInput = false;
        }

        if (okInput)
        {
            _inpKgrmKgm = v;
        }

        return okInput;
    }

    private void textboxInpKg_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CheckKiloInput())
        {
            _inputKg = _inpKgrmKgm;
            DisplayValues();
            ButtonSave.IsEnabled = true;    
        }
    }

    private void buttonSave_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void buttonCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}