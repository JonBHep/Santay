using System;

namespace Santay;

public class Epoch : IComparable<Epoch>
{
    // grouping of rides by time or location
    private DateOnly _firstDate;
    public string Caption { get; }

    public DateOnly FirstDate { get => _firstDate; set => _firstDate = value; }
    public DateOnly LastDate { get; set; }

    public Epoch(string caption)
    {
        Caption = caption;
        _firstDate = DateOnly.MaxValue;
        LastDate = DateOnly.MinValue;;
    }

    public int CompareTo(Epoch? other)
    {
        return other is { } ? _firstDate.CompareTo(other._firstDate) : 0;
    }
}