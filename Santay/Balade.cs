using System;

namespace Santay;

public class Balade : IComparable<Balade>
{
    
    private readonly DateTime _jour;
    private readonly DateOnly _dateOnly;
    private readonly double _kilometresJ;
    private readonly string _caption;
    private readonly string _group;
    private readonly int _difficulty;

    public Balade(string specifier)
    {
        string[] s = specifier.Split("@".ToCharArray());
        long a = long.Parse(s[0]);
        DateTime dt = DateTime.FromBinary(a);
        _jour = dt.Date;
        _dateOnly= DateOnly.FromDateTime(_jour);
        _kilometresJ = double.Parse(s[1]);
        _caption = s[2];
        _group = s[3];
        _difficulty = int.Parse(s[4]);
    }

    public string Specification => $"{_jour.Date.ToBinary()}@{_kilometresJ}@{_caption}@{_group}@{_difficulty}@"; 

    public Balade(DateTime dat, double kmJ, string cp, string gp, int diff)
    {
        _kilometresJ = kmJ;
        _jour = dat;
        _caption = cp;
        _group = gp;
        _difficulty = diff;
    }
    public Balade(DateOnly dat, double kmJ, string cp, string gp, int diff)
    {
        _kilometresJ = kmJ;
        _dateOnly = dat;
        //_jour = dat;
        _caption = cp;
        _group = gp;
        _difficulty = diff;
    }

    public string RideCaption => _caption;

    public string RideGroup => _group;

    public int Difficulty => _difficulty;

   // public DateTime RideDate => _jour.Date;
    public DateOnly TripDate => _dateOnly;

    public double RideKm => _kilometresJ;

    public string RideKmStringJbh => (_kilometresJ > 0) ? _kilometresJ.ToString("0.00 km") : "-";

    int IComparable<Balade>.CompareTo(Balade? other)
    {
        return other is null ? 0 : _jour.CompareTo(other._jour);
    }

    public static string DifficultyCaption(int diff)
    {
        switch (diff)
        {
            case 1:
            {
                return "Easy & flat";
            }
            case 3:
            {
                return "Difficult";
            }
            default:
            {
                return "Intermediate";
            }
        }
    }

}