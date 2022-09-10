using System;

namespace Santay;

public class Gearing : IComparable<Gearing>
{
    private readonly int _chainringIndex;
    private readonly int _sprocketIndex;

    private readonly double _ratio;
    public Gearing(int cindex, int cteeth, int sindex, int steeth)
    {
        _chainringIndex = cindex;
        _sprocketIndex = sindex;
        SprocketTeeth = steeth;
        _ratio = (double)cteeth / SprocketTeeth;
    }

    private int SprocketTeeth { get; }
    public double Ratio => _ratio;
    public int ChainringIndex => _chainringIndex;
    public int SprocketIndex => _sprocketIndex;

    public bool IsCrossover(int chainrings, int sprockets)
    {
        // non-advised gearings where chain crosses sides on chainrings and sprockets
        return IsCrossover(chainrings, sprockets, _chainringIndex, _sprocketIndex);
    }

    public static bool IsCrossover(int chainringCount, int sprocketCount, int chainringIndex, int sprocketIndex)
    {
        // non-advised gearings where chain crosses sides on chainrings and sprockets
        bool ok = true;
        double midchainring = (chainringCount + 1) / 2D;
        double midsprocket = (sprocketCount + 1) / 2D;
        if ((chainringIndex < midchainring) && (sprocketIndex > midsprocket)) { ok = false; }
        if ((chainringIndex > midchainring) && (sprocketIndex < midsprocket)) { ok = false; }
        return !ok;
    }

    int IComparable<Gearing>.CompareTo(Gearing? other)
    {
        return other is { } ? _ratio.CompareTo(other._ratio) : 0;
    }
}