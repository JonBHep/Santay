namespace Santay;

public class EpochStats : GroupedStats
{
    public EpochStats(Epoch whenWhere)
    {
        string dates;
        if (whenWhere.FirstDate.Year.Equals(whenWhere.LastDate.Year))
        {
            if (whenWhere.FirstDate.Month.Equals(whenWhere.LastDate.Month))
            {
                dates = whenWhere.FirstDate.Day.Equals(whenWhere.LastDate.Day) ? $"({whenWhere.FirstDate:dd MMM yyyy})" : $"({whenWhere.FirstDate.ToString("dd")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
            }
            else
            {
                // same year
                dates = $"({whenWhere.FirstDate.ToString("dd MMM")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
            }
        }
        else
        {
            // different year
            dates = $"({whenWhere.FirstDate.ToString("dd MMM yyyy")} to {whenWhere.LastDate.ToString("dd MMM yyyy")})";
        }

        GroupCaption = $"{whenWhere.Caption} {dates}";
GroupKey = whenWhere.Caption;
    }

    public void ConsiderRide(Balade outing, int rideIdent)
    {
        if (outing.RideGroup != GroupKey)
        {
            return;
        }

        this.AddRide(outing, rideIdent);
    }

    public string GroupCaption { get; }

    private string GroupKey { get; set; }

    // public VeloHistory.TripType TripKind
    // {
    //     get
    //     {
    //         if (TripCountPied > 0)
    //         {
    //             return VeloHistory.TripType.Walk;
    //         }
    //
    //         return VeloHistory.TripType.Cycle;
    //     }
    // }

    // public bool MixedTripKinds => ((TripCountPied > 0) && (TripCountVelo > 0));
}