using System;

namespace Santay;

public class MonthlyStats : GroupedStats
{
    private readonly DateOnly _anyDay;

    public MonthlyStats(DateOnly anyDate)
    {
        _anyDay = anyDate; // any date within the target month
    }

    public void ConsiderRide(Balade outing, int rideIdent)
    {
        if (outing.TripDate.Year != _anyDay.Year) { return; }
        if (outing.TripDate.Month != _anyDay.Month) { return; }
        this.AddRide(outing, rideIdent);
    }

    public int YearNumber => _anyDay.Year;
    public int MonthNumber => _anyDay.Month;
}