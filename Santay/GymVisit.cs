using System;

namespace Santay;

public class GymVisit : IComparable<GymVisit>
{
    public GymHistory.GymType Activity { get; set; }
    public DateTime When { get; set; }
    public int Index { get; set; }
    public string Specification
    {
        get
        {
            string wot = ((int)Activity).ToString();
            string wen = StringFromDate(When);
            return wen + wot;
        }
        init
        {
            int wot = int.Parse(value.Substring(8, 1));
            Activity = (GymHistory.GymType)wot;
            When = DateFromString(value.Substring(0, 8));
        }
    }

    public string WhenCode => StringFromDate(When);

    int IComparable<GymVisit>.CompareTo(GymVisit? other)
    {
        return other is { } ? this.When.CompareTo(other.When) : 0;
    }

    private DateTime DateFromString(string code)
    {
        int y = int.Parse(code.Substring(0, 4));
        int m = int.Parse(code.Substring(4, 2));
        int d = int.Parse(code.Substring(6, 2));
        return new DateTime(y, m, d);
    }

    public static string StringFromDate(DateTime dt)
    {
        string y = dt.Year.ToString("0000");
        string m = dt.Month.ToString("00");
        string d = dt.Day.ToString("00");
        return y + m + d;
    }
}