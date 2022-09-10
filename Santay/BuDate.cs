using System;

namespace Santay;

public class BuDate
{
    private static readonly DateTime Birth = new DateTime(1954, 1, 3);

    public int QuandInteger { get; init; }
    public Ivresse Quoi { get; init; }

    public enum Ivresse { SaisPas = 0, Bu = 1, PasBu = 2 };

    public static int DayOfLife(DateTime target)
    {
        TimeSpan ts = target - Birth;
        return (int)(ts.TotalDays + 1);
    }

    private static DateTime DateFromDayOfLife(int dayOfLife)
    {
        int days = dayOfLife - 1;
        return Birth.AddDays(days);
    }

    public string Specification
    {
        get => $"{QuandInteger:00000}{(int)Quoi}";
        init
        {
            string quoistr = value.Substring(5, 1);
            if (int.TryParse(quoistr, out int i))
            {
                Quoi = (Ivresse)i;
            }
            else
            {
                Quoi = Ivresse.SaisPas;
            }
            string quandstr = value.Substring(0, 5);
            QuandInteger = int.TryParse(quandstr, out int j) ? j : 0;
        }
    }

    public static string DateStamp(int a)
    {
        DateTime dt = DateFromDayOfLife(a);
        return $"{dt:d}"; // short date
    }
}