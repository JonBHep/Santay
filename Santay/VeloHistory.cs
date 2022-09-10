using System;
using System.Collections.Generic;
using Jbh;

namespace Santay;

public class VeloHistory
{

    private readonly List<Balade> _trips = new List<Balade>();
    private readonly string _veldatafile;
    private RollingMonthStats _rollingMonthStats;
    private int _outingsV;
    private double _averageKmPerTripV;
    private double _averageKmPerDayV;
    private int _totalDaysV;
    private double _maxTripKmV;
    private double _totalKmV;
    private DateOnly _dateEarliestRide;
    private DateOnly _dateLatestRide;

    private DateOnly _dateFirstV = DateOnly.MaxValue;
    public static System.Windows.Media.Brush BrushEasy => System.Windows.Media.Brushes.DarkSeaGreen;

    public static System.Windows.Media.Brush BrushIntermediate =>
        new System.Windows.Media.SolidColorBrush(ColourMix(System.Windows.Media.Colors.DarkSeaGreen
            , System.Windows.Media.Colors.LightCoral, 0.5));

    public static System.Windows.Media.Brush BrushHard => System.Windows.Media.Brushes.LightCoral;

    public VeloHistory(string vfile)
    {
        _veldatafile = vfile;
        string vDatafile = AppManager.DataPath;
        vDatafile = System.IO.Path.Combine(vDatafile, _veldatafile);
        if (System.IO.File.Exists(vDatafile))
        {
            using var sr = new System.IO.StreamReader(vDatafile);
            while (!sr.EndOfStream)
            {
                var j = sr.ReadLine();
                if (j is { })
                {
                    Balade b = new Balade(j);
                    _trips.Add(b);
                }
            }
        }

        _rollingMonthStats = new RollingMonthStats(NotionalNow);
        Recalculate();
    }

    public void SaveData()
    {
        // get data file path
        string vDatafile = AppManager.DataPath;
        vDatafile = System.IO.Path.Combine(vDatafile, _veldatafile);
        // backup existing data
        AppManager.CreateBackupDataFile(vDatafile);
        AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(_veldatafile), minimumDaysToKeep: 40
            , minimumFilesToKeep: 4);

        // write new data
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(vDatafile);
        foreach (Balade b in _trips)
        {
            // if (b.Kind == TripType.Cycle)
            // {
                sw.WriteLine(b.Specification);    
            // }
        }
    }

    public int TripCountCycling => _trips.Count;

    private void Recalculate()
    {
        _outingsV = 0;
        _maxTripKmV = 0;
        _totalKmV = 0;
        _dateEarliestRide = DateOnly.MaxValue;
        _dateLatestRide = DateOnly.MinValue;
        if (_trips.Count < 1)
        {
            return;
        }

        _rollingMonthStats = new RollingMonthStats(NotionalNow);

        _trips.Sort();
        int z = 0;
        foreach (Balade b in _trips)
        {
            _rollingMonthStats.ConsiderRide(b, z);
            double j = b.RideKm;
            if (j > 0)
            {
                _outingsV++;
                _maxTripKmV = Math.Max(_maxTripKmV, j);
                _totalKmV += j;
            }
            if (_outingsV > 0)
            {
                _averageKmPerTripV = _totalKmV / _outingsV;
            }

            if (b.TripDate < _dateEarliestRide)
            {
                _dateEarliestRide = b.TripDate;
            }
            if (b.TripDate > _dateLatestRide)
            {
                _dateLatestRide = b.TripDate;
            }
            z++;
        }

        _dateFirstV = DateOnly.MaxValue;
        foreach (Balade b in _trips)
        {
            if (_dateFirstV > b.TripDate)
            {
                _dateFirstV = b.TripDate;
            }
        }

        int t = NotionalNow.DayNumber - _dateFirstV.DayNumber;
        double daysSpan = 1 + t;
        _totalDaysV = (int) daysSpan;
        _averageKmPerDayV = _totalKmV / daysSpan;
    }

    public double PeriodDistance(DateOnly first, DateOnly last)
    {
        double accum = 0;
        foreach (Balade b in _trips)
        {
            if ((b.TripDate >= first) && (b.TripDate <= last))
            {
                accum += b.RideKm;
            }
        }

        return accum;
    }

    public int PeriodTripCount(DateOnly first, DateOnly last)
    {
        int counted = 0;
        foreach (Balade b in _trips)
        {
            if ((b.TripDate >= first) && (b.TripDate <= last))
            {
                counted++;
            }
        }

        return counted;
    }
    public RollingMonthStats RollingMonth => _rollingMonthStats;

    public double RollingYearMonthlyMeanKm(int annee, int mois)
    {
        DateOnly g = new DateOnly(annee, mois, 1);
        g = g.AddMonths(1);
        g = g.AddDays(-1);
        DateOnly yearEnd = g;
        DateOnly yearStart = yearEnd.AddYears(-1);
        double yearKm = PeriodDistance(yearStart.AddDays(1), yearEnd);
        return yearKm / 12;
    }

    public DateOnly HistoryFirstDate
    {
        get
        {
            
                return _dateFirstV;
            
        }
    }

    public int DistanceRanking(double distance)
    {
        int rnk = 1;
        foreach (Balade b in _trips)
        {
            if (b.RideKm > distance)
            {
                rnk++;
            }
        }

        return rnk;
    }

    public void AddTrip(Balade newBallade)
    {
        _trips.Add(newBallade);
        Recalculate();
    }

    public void RemoveTripOnDate(DateOnly d)
    {
        int z = -1;
        for (int i = 0; i < _trips.Count; i++)
        {
            if (_trips[i].TripDate.Equals(d))
            {
                z = i;
                break;
            }
        }

        if (z >= 0)
        {
            _trips.RemoveAt(z);
        }

        Recalculate();
    }

    public double MaximumTripKmVelo => _maxTripKmV;
    public DateOnly EarliestRide => _dateEarliestRide;
    public DateOnly LatestRide => _dateLatestRide;
    
    public double AverageTripKmVelo => _averageKmPerTripV;

    public double AverageDailyKmVelo => _averageKmPerDayV;

    public double Average4WeeklyKmVelo => _averageKmPerDayV * 28;

    public double TotalDistanceKmV => _totalKmV;

    public int TotalDaysCycling => _totalDaysV;

    
    public static double MilesFromKm(double km)
    {
        return km * 0.621371192;
    }

    // public static double KmFromMiles(double miles)
    // {
    //     return miles * 1.609344;
    // }

    
    public DateOnly NotionalNow
    {
        get
        {
            DateOnly aujourdhui = DateOnly.FromDateTime(DateTime.Today);
            
            DateOnly nto =aujourdhui;
            // If there is no ride recorded for today, take stats up to yesterday rather than today, as we don't want to reduce the averages when there could still be a trip recorded today. We assume there isn't still a trip to be recorded for yesterday!
            if (_trips.Count < 1)
            {
                return nto;
            }

            if (!_trips[^1].TripDate.Equals(aujourdhui))
            {
                nto =aujourdhui.AddDays(-1);
            }

            return nto;
        }
    }
    
    public Balade? TripOnDate(DateTime q)
    {
        DateOnly qd =DateOnly.FromDateTime(q);
        Balade? rv = null;
        foreach (Balade r in _trips)
        {
            if (r.TripDate.Equals(qd))
            {
                rv = r;
            }
        }

        return rv;
    }
    public Balade? TripOnDate(DateOnly q)
    {
        //DateTime qd = q.Date;
        Balade? rv = null;
        foreach (Balade r in _trips)
        {
            if (r.TripDate.Equals(q))
            {
                rv = r;
            }
        }

        return rv;
    }

    public static string Ordinal(int r)
    {
        string v = r.ToString();
        int tens = r % 100;
        int units = tens % 10;
        tens /= 10;
        switch (tens)
        {
            case 1:
            {
                v += "th";
                break;
            }
            default:
            {
                switch (units)
                {
                    case 1:
                    {
                        v += "st";
                        break;
                    }
                    case 2:
                    {
                        v += "nd";
                        break;
                    }
                    case 3:
                    {
                        v += "rd";
                        break;
                    }
                    default:
                    {
                        v += "th";
                        break;
                    }
                }

                break;
            }
        }

        return v;
    }

    /// <summary>
    /// Returns a System.Windows.Media.Color that is a mix of the first and second given Colors in the specified proportion
    /// </summary>
    /// <param name="first">First colour</param>
    /// <param name="second">Second colour</param>
    /// <param name="mix">Proportion of second colour to mix with first colour (0 = 100% first colour, 1 = 100% second colour)</param>
    /// <returns></returns>
    public static System.Windows.Media.Color ColourMix(System.Windows.Media.Color first
        , System.Windows.Media.Color second, double mix)
    {
        byte redF = first.R;
        byte grnF = first.G;
        byte bluF = first.B;
        byte redS = second.R;
        byte grnS = second.G;
        byte bluS = second.B;
        double fMix = 1 - mix;
        double redMix = (fMix * redF) + (mix * redS);
        double grnMix = (fMix * grnF) + (mix * grnS);
        double bluMix = (fMix * bluF) + (mix * bluS);
        byte redM = (byte) (redMix + 0.5); // Adding 0.5 because (int) rounds down
        byte grnM = (byte) (grnMix + 0.5);
        byte bluM = (byte) (bluMix + 0.5);
        return System.Windows.Media.Color.FromRgb(redM, grnM, bluM);
    }

    public List<string> GroupList
    {
        get
        {
            List<string> groups = new List<string>();
            foreach (Balade r in _trips)
            {
                if (!string.IsNullOrWhiteSpace(r.RideGroup))
                {
                    if (!groups.Contains(r.RideGroup))
                    {
                        groups.Add(r.RideGroup);
                    }
                }
            }

            groups.Sort();
            return groups;
        }
    }

    public int TripCountV => _outingsV;
    public int TripCountAll => _trips.Count;
    public List<Balade> TripList => _trips;
}