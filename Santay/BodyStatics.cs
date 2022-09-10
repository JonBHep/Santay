using System;

namespace Santay;

public static class BodyStatics
{
    private const double LowIdealBmi = 18.5;
    private const double HighIdealBmi = 25;

    public static double WeightAsPounds(double kgs)
    {
        return kgs / 0.45359237;
    }

    private static double HeightAsInches(double Mtrs)
    {
        double cm = Mtrs * 100;
        return cm / 2.54;
    }

    public static string WeightAsStonesAndPoundsString(double kgs)
    {
        double lb = WeightAsPounds(kgs);
        lb = Math.Round(lb);
        int lbs = (int)lb;
        int s = (int)Math.Floor(lbs / 14f);
        int p = lbs % 14;
        return s.ToString() + " stone " + p.ToString() + " lb";
    }

    public static string HeightAsFeetAndInchesString(double mtrs)
    {
        double inches = HeightAsInches(mtrs);
        int feet = (int)Math.Floor(inches / 12f);
        int usedinches = feet * 12;
        double ins = inches - usedinches;
        return $"{feet} ft {ins:0.0} in";
    }

    public static double BmiOf(double kg, double myHeightInMetres)
    { return (float)(kg / (Math.Pow(myHeightInMetres, 2))); }

    private static double WeightInKgForBmi(double bmi, double myHeightInMetres)
    {
        return bmi * Math.Pow(myHeightInMetres, 2);
    }

    public static double IdealWeightLowerLimitKg(double heightInMetres) { return WeightInKgForBmi(LowIdealBmi, heightInMetres); }

    public static double IdealWeightHigherLimit(double heightInMetres) { return WeightInKgForBmi(HighIdealBmi, heightInMetres); }

    public static double TargetWaist => 100;

    public static int CountDays(DateTime start, DateTime finish)
    {
        TimeSpan ts = finish.Subtract(start);
        return (int)ts.TotalDays;
    }

}