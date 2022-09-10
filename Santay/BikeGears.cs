using System.Collections.Generic;
using Jbh;

namespace Santay;

public class BikeGears
{
    
    private List<Gearing> _gearings = new List<Gearing>();
    private string _bikeName;
    private int _wheelCircumferenceMm;
    private List<int> _chainrings = new List<int>();
    private List<int> _sprockets = new List<int>();

    public string BikeName
    {
        get => _bikeName;
        set => _bikeName = value;
    }

    public List<Gearing> Gearings
    {
        get => _gearings;
        set => _gearings = value;
    }

    public int WheelCircumference
    {
        get => _wheelCircumferenceMm;
        set => _wheelCircumferenceMm = value;
    }

    public List<int> Chainrings
    {
        get => _chainrings;
        set => _chainrings = value;
    }

    public List<int> Sprockets
    {
        get => _sprockets;
        set => _sprockets = value;
    }

    public BikeGears(string cycleName)
    {
        _bikeName = cycleName;
    }

    public static List<int> CogWheelTeeth(string codeString)
    {
        string[] rings = codeString.Split("-".ToCharArray());
        List<int> teeth = new List<int>();
        bool fail = false;
        foreach (string part in rings)
        {
            if (int.TryParse(part, out int t))
            {
                teeth.Add(t);
            }
            else
            {
                fail = true;
            }
        }

        if (fail)
        {
            teeth = new List<int>();
        }

        return teeth;
    }

    public static string CogwheelSpecificationString(List<int> sourceList)
    {
        return string.Join("-", sourceList);
    }

    public void Load()
    {
        string filepath = System.IO.Path.Combine(AppManager.DataPath, _bikeName + ".jbh");
        using System.IO.StreamReader sr = new System.IO.StreamReader(filepath);
        
        var s = sr.ReadLine();
        if (s is { })
        {
            _chainrings = CogWheelTeeth(s);    
        }
        s = sr.ReadLine();
        if (s is { })
        {
            _sprockets = CogWheelTeeth(s);    
        }
        s = sr.ReadLine();
        if (s is { })
        {
            _wheelCircumferenceMm = int.Parse(s);    
        }
    }

    public void Save()
    {
        string filepath = System.IO.Path.Combine(AppManager.DataPath, _bikeName + ".jbh");
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(filepath);
        sw.WriteLine(CogwheelSpecificationString(_chainrings));
        sw.WriteLine(CogwheelSpecificationString(_sprockets));
        sw.WriteLine(_wheelCircumferenceMm.ToString());
    }

    public void CalculateGearings()
    {
        _gearings = new List<Gearing>();
        for (int c = 0; c < _chainrings.Count; c++)
        {
            for (int s = 0; s < _sprockets.Count; s++)
            {
                Gearing g = new Gearing(c + 1, _chainrings[c], s + 1, _sprockets[s]);
                _gearings.Add(g);
            }

            _gearings.Sort();
        }
    }

}
