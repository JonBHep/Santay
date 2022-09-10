using System.Collections.Generic;
using Jbh;

namespace Santay;

public class GymHistory
{
    public enum GymType { GymTraining, AquaAerobics, Other, Void };

    private readonly List<GymVisit> _gymVisits = new List<GymVisit>();
    private readonly string _gymdatafile;
    public List<GymVisit> GymList => _gymVisits;

    public GymHistory(string gfile)
    {
        // get data file path
        _gymdatafile = gfile;
        string gDatafile = AppManager.DataPath;
        gDatafile = System.IO.Path.Combine(gDatafile, _gymdatafile);
        if (System.IO.File.Exists(gDatafile))
        {
            using System.IO.StreamReader sr = new System.IO.StreamReader(gDatafile);
            while (!sr.EndOfStream)
            {
                var s = sr.ReadLine();
                if (s is { })
                {
                    GymVisit g = new GymVisit() {Specification = s};
                    _gymVisits.Add(g);
                }
            }
        }
    }

    public void SaveData()
    {
        string gDatafile = AppManager.DataPath;
        gDatafile = System.IO.Path.Combine(gDatafile, _gymdatafile);
        // backup existing data
        AppManager.CreateBackupDataFile(gDatafile);
        AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(_gymdatafile), minimumDaysToKeep: 40, minimumFilesToKeep: 4);

        // write new data
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(gDatafile);
        foreach (GymVisit g in _gymVisits)
        {
            sw.WriteLine(g.Specification);
        }
    }
}