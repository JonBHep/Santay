using System;

namespace Santay;

public class RollingMonthStats : GroupedStats
{
    readonly DateOnly _monthAgo;

    public RollingMonthStats(DateOnly notionalToday)
    {
        _monthAgo = notionalToday.AddMonths(-1);
    }

    public void ConsiderRide(Balade outing, int rideIdent)
    {
        if (outing.TripDate <= _monthAgo) { return; }
        AddRide(outing, rideIdent);
    }
}