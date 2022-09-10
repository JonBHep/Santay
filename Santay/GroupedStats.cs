using System;
using System.Collections.Generic;

namespace Santay;

public abstract class GroupedStats
{
    private double _minTripKmVelo;
    private double _maxTripKmVelo;

    protected GroupedStats()
    {
        TripCountVelo = 0;
        RiddenKilometres = 0;
        RideIdentifiers = new List<int>();
        WalkedKilometres = 0;
        _maxTripKmVelo = 0;
        _minTripKmVelo = double.MaxValue;
    }

    protected void AddRide(Balade outing, int rideId)
    {
        TripCountVelo++;
        RiddenKilometres += outing.RideKm;
        _minTripKmVelo = Math.Min(_minTripKmVelo, outing.RideKm);
        _maxTripKmVelo = Math.Max(_maxTripKmVelo, outing.RideKm);
        RideIdentifiers.Add(rideId);
    }

    public int TripCountVelo { get; private set; }
    public double RiddenKilometres { get; private set; }
    public double WalkedKilometres { get; private set; }
    public List<int> RideIdentifiers { get; }

    public int PerTripKmMinVelo => (int) Math.Round(_minTripKmVelo);

    public int PerTripKmMaxVelo => (int) Math.Round(_maxTripKmVelo);

    public int PerTripKmMeanVelo => (int) Math.Round(RiddenKilometres / TripCountVelo);
}