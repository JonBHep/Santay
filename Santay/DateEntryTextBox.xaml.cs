using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Santay;

public partial class DateEntryTextBox
{
    private DateTime? _quand;
    //private bool _hasValue;
    // public event EventHandler ValueChanged;

    public DateEntryTextBox()
    {
        InitializeComponent();
        // ValueChanged = new();
        Clear();
    }

    private void textboxDate_TextChanged(object sender, TextChangedEventArgs e)
    {
        string q = TextboxDate.Text.Trim();
        if (string.IsNullOrWhiteSpace(q))
        {
            TextboxDate.Opacity = 0.5;
            // _hasValue = false;
            _quand = null;
            TextboxDate.ToolTip = "Null date";
            TextblockResult.Text = "Null date";
        }
        else
        {
            TextboxDate.Opacity = 1;
            if (DateTime.TryParse(q, out var u))
            {
                TextboxDate.Foreground = Brushes.Black;
                _quand = u;
                TextboxDate.ToolTip = u.ToString("dd MMM yyyy");
                TextblockResult.Text = u.ToString("dd MMM yyyy");
            }
            else
            {
                TextboxDate.Foreground = Brushes.Red;
                _quand = null;
                TextboxDate.ToolTip = "Null date";
                TextblockResult.Text = "Null date";
            }
        }
    }

    public DateTime? DateValue
    {
        get
        {
            if (_quand is { })
            {
                return _quand;
            }

            return null;
        }
        set
        {
            _quand = value;
            if (_quand is { } w)
            {
                if (w.Ticks < 1)
                {
                    TextboxDate.Clear();
                }
                else
                {
                    TextboxDate.Text = w.ToShortDateString();
                }
            }
            else
            {
                TextboxDate.Clear();
            }
        }
    }


    private void Clear()
    {
        _quand = null;
        TextboxDate.Clear();
    }

    private void textboxDate_GotFocus(object sender, RoutedEventArgs e)
    {
        GridBase.Background = Brushes.White;
    }

    private void textboxDate_LostFocus(object sender, RoutedEventArgs e)
    {
        GridBase.Background = Brushes.WhiteSmoke;
    }

    private void UserControl_Initialized(object sender, EventArgs e)
    {
        TextblockResult.Text = string.Empty;
    }

}