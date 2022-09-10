using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Santay;

public partial class EnterBloodPressureWindow
{
    private int _dataDia;
    private int _dataPulse;
    private int _dataSys;
    
    public int RvDiastolic => _dataDia;
    public int RvPulse => _dataPulse;
    public int RvSystolic => _dataSys;
    public EnterBloodPressureWindow()
    {
        InitializeComponent();
    }

    private void textboxSystolic_TextChanged(object sender, TextChangedEventArgs e)
    {
        ButtonSave.IsEnabled = InputDataIsOk();
    }

    public DateTime ReadingDate
    {
        get
        {
            DateTime q = DateTime.Today;
            if (DateTextBloc.Tag is DateOnly dOnly)
            {
                q = new DateTime(dOnly.Year, dOnly.Month, dOnly.Day);
            }

            if (TimeTextBloc.Tag is TimeOnly tOnly)
            {
                var tim= TimeSpan.FromTicks(tOnly.Ticks);
                q = q.Add(tim);
            }

            return q;
        }
    }

    private bool InputDataIsOk()
    {
        if (DateTextBloc.Tag is null)
        {
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(TextboxDiastolic.Text))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(TextboxPulse.Text))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(TextboxSystolic.Text))
        {
            return false;
        }

        if (!int.TryParse(TextboxDiastolic.Text, out _dataDia))
        {
            return false;
        }

        if (!int.TryParse(TextboxPulse.Text, out _dataPulse))
        {
            return false;
        }

        if (!int.TryParse(TextboxSystolic.Text, out _dataSys))
        {
            return false;
        }

        return true;
    }
  
    private void textboxDiastolic_TextChanged(object sender, TextChangedEventArgs e)
    {
        ButtonSave.IsEnabled = InputDataIsOk();
    }

    private void textboxPulse_TextChanged(object sender, TextChangedEventArgs e)
    {
        ButtonSave.IsEnabled = InputDataIsOk();
    }

    public void PopulateWith(DateTime  bpDate, int bpDiastolic, int bpPulse, int bpSystolic)
    {
        DateTextBox.Text = bpDate.ToShortDateString();
        TimeTextBox.Text = bpDate.ToShortTimeString();
        
        TextboxDiastolic.Text = bpDiastolic.ToString();
        TextboxPulse.Text = bpPulse.ToString();
        TextboxSystolic.Text = bpSystolic.ToString();
        ButtonSave.IsEnabled = false;
    }

    private void buttonSave_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
    
    private void DateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var entered = DateTextBox.Text.Trim();
        if (DateOnly.TryParse(entered, out var myDate))
        {
            DateTextBloc.Text = myDate.ToLongDateString();
            DateTextBloc.Foreground=Brushes.Green;
            DateTextBloc.Tag = myDate;
        }
        else
        {
            DateTextBloc.Text = "?";
            DateTextBloc.Foreground=Brushes.Red;
            DateTextBloc.Tag = null;
        }
        ButtonSave.IsEnabled = InputDataIsOk();
    }
    

    private void TimeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var entered = TimeTextBox.Text.Trim();
        if (TimeOnly.TryParse(entered, out var myTime))
        {
            TimeTextBloc.Text = myTime.ToShortTimeString();
            TimeTextBloc.Foreground=Brushes.Green;
            TimeTextBloc.Tag = myTime;
        }
        else
        {
            TimeTextBloc.Text = "?";
            TimeTextBloc.Foreground=Brushes.Red;
            TimeTextBloc.Tag = null;
        }
        ButtonSave.IsEnabled = InputDataIsOk();
    }

    private void TextBoxNumeric_OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox box)
        {
            if (box.Text == "0")
            {
                box.Text = string.Empty;
            }
        }
    }
}