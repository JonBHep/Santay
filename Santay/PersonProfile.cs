using System;
using System.Collections.Generic;
using System.Globalization;
using Jbh;

namespace Santay;

public class PersonProfile
{
    public PersonProfile()
    {
        // _waistReadings = new List<Waist>();
        _weightReadings = new List<Weight>();
        _tensionReadings = new List<Tension>();
        LoadHeightData();
        LoadBloodPressureData();
        LoadWeightData();
        // LoadWaistData();
    }
    
    public class Tension : IComparable<Tension>
    {
        public DateTime BprWhen { get; set; }
        public int BpDiastolic { get; set; }
        public int BpSystolic { get; set; }
        public int Pulse { get; set; }

        public int CompareTo(Tension? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return BprWhen.CompareTo(other.BprWhen);
        }
    }

    public List<Tension> DayTensions(DateOnly when)
    {
        var tList = new List<Tension>();
        foreach (var reading in _tensionReadings)
        {
            if (SameDay(reading.BprWhen, when))
            {
                tList.Add(reading);
            }
        }

        return tList;
    }
    
    public Weight? DayWeight(DateOnly when)
    {
        foreach (var reading in _weightReadings)
        {
            if (reading.WgtWhen.Equals(when))
            {
                return reading;
            }
        }

        return null;
    }
    
    private bool SameDay(DateTime dt, DateOnly dy)
    {
        if (dt.Year != dy.Year)
        {
            return false;
        }
        if (dt.Month != dy.Month)
        {
            return false;
        }
        if (dt.Day != dy.Day)
        {
            return false;
        }

        return true;
    }
    // public class Waist : IComparable
    // {
    //     public DateOnly WstWhen { get; set; }
    //     public double WstCentimetres { get; set; }
    //
    //     public int CompareTo(object? other)
    //     {
    //         if (other is Waist w)
    //         {
    //             return WstWhen.CompareTo(w.WstWhen);
    //         }
    //
    //         return 0;
    //     }
    // }

    public class Weight : IComparable
    {
        public DateOnly WgtWhen { get; set; }
        public double WgtKilograms { get; set; }

        public int CompareTo(object? other)
        {
            if (other is Weight wt)
            {
                return WgtWhen.CompareTo((wt.WgtWhen));
            }

            return 0;
        }
    }

    // private readonly List<BloodPressure> _bloodPressureReadings;
    private readonly List<Tension> _tensionReadings;
    // private readonly List<Waist> _waistReadings;
    private readonly List<Weight> _weightReadings;

    private double _heightInMetres;

    private void SaveBloodPressureData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.tension");
        AppManager.CreateBackupDataFile(dataFile);
        AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(dataFile), minimumDaysToKeep: 5
            , minimumFilesToKeep: 5);
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile);
        foreach (Tension t in _tensionReadings)
        {
            sw.WriteLine($"{t.BprWhen.ToBinary()}");
            sw.WriteLine($"{t.BpSystolic}");
            sw.WriteLine($"{t.BpDiastolic}");
            sw.WriteLine($"{t.Pulse}");
        }
    }
    private void LoadBloodPressureData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.tension");
        if (!System.IO.File.Exists(dataFile))
        {
            return;
        }

        using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
        {
            while (!sr.EndOfStream)
            {
                //BloodPressure r = new BloodPressure();
                Tension t = new Tension();
                string? j = sr.ReadLine(); 
                if (j is { })
                {
                    if (long.TryParse(j, out var x))
                    {
                        // r.BprWhen = DateOnly.FromDayNumber(x);
                        t.BprWhen =DateTime.FromBinary(x);
                    }
                }

                j = sr.ReadLine();
                if (j is { })
                {
                    t.BpSystolic = int.Parse(j);
                }

                j = sr.ReadLine();
                if (j is { })
                {
                    t.BpDiastolic = int.Parse(j);
                }

                j = sr.ReadLine();
                if (j is { })
                {
                    t.Pulse = int.Parse(j);
                }

                _tensionReadings.Add(t);
            }
        }

        // _bloodPressureReadings.Sort();
        _tensionReadings.Sort();
    }

    private void LoadHeightData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.height");
        if (!System.IO.File.Exists(dataFile))
        {
            return;
        }

        using System.IO.StreamReader sr = new System.IO.StreamReader(dataFile);
        var j = sr.ReadLine();
        if (j is { })
        {
            HeightInMetres = double.Parse(j);
        }
    }

    // private void LoadWaistData()
    // {
    //     string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.waist");
    //     if (!System.IO.File.Exists(dataFile))
    //     {
    //         return;
    //     }
    //
    //     using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
    //     {
    //         while (!sr.EndOfStream)
    //         {
    //             Waist r = new Waist();
    //
    //             var j = sr.ReadLine(); 
    //             if (j is { })
    //             {
    //                 if (int.TryParse(j, out int x))
    //                 {
    //                     r.WstWhen = DateOnly.FromDayNumber(x);
    //                 }
    //             }
    //
    //             j = sr.ReadLine();
    //             if (j is { })
    //             {
    //                 r.WstCentimetres = double.Parse(j);
    //             }
    //
    //             _waistReadings.Add(r);
    //         }
    //     }
    //
    //     _waistReadings.Sort();
    // }

    private void LoadWeightData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.weight");
        if (!System.IO.File.Exists(dataFile))
        {
            return;
        }

        using (System.IO.StreamReader sr = new System.IO.StreamReader(dataFile))
        {
            while (!sr.EndOfStream)
            {
                Weight r = new Weight();
                var j = sr.ReadLine(); 
                if (j is { })
                {
                    if (int.TryParse(j, out int x))
                    {
                        r.WgtWhen = DateOnly.FromDayNumber(x);
                    }
                }

                j = sr.ReadLine();
                if (j is { })
                {
                    r.WgtKilograms = double.Parse(j);
                }

                _weightReadings.Add(r);
            }
        }

        _weightReadings.Sort();
    }

    private void SaveHeightData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.height");
        AppManager.CreateBackupDataFile(dataFile);
        AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(dataFile), minimumDaysToKeep: 20
            , minimumFilesToKeep: 4);
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile);
        sw.WriteLine(HeightInMetres.ToString(CultureInfo.InvariantCulture));
    }

    

    // private void SaveWaistData()
    // {
    //     string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.waist");
    //     AppManager.CreateBackupDataFile(dataFile);
    //     AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(dataFile), minimumDaysToKeep: 20
    //         , minimumFilesToKeep: 4);
    //     using System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile);
    //     foreach (Waist r in _waistReadings)
    //     {
    //         //  sw.WriteLine(r.WstDate.ToString(CultureInfo.CurrentCulture));
    //         // r.WstWhen=DateOnly.FromDateTime(r.WstDate);
    //         sw.WriteLine(r.WstWhen.DayNumber.ToString());
    //         sw.WriteLine(r.WstCentimetres.ToString(CultureInfo.InvariantCulture));
    //     }
    // }

    private void SaveWeightData()
    {
        string dataFile = System.IO.Path.Combine(AppManager.DataPath, "Data.weight");
        AppManager.CreateBackupDataFile(dataFile);
        AppManager.PurgeOldBackups(fileExtension: System.IO.Path.GetExtension(dataFile), minimumDaysToKeep: 20
            , minimumFilesToKeep: 4);
        using System.IO.StreamWriter sw = new System.IO.StreamWriter(dataFile);
        foreach (Weight r in _weightReadings)
        {
            // sw.WriteLine(r.WgtDate.ToString(CultureInfo.InvariantCulture));
            // r.WgtWhen=DateOnly.FromDateTime(r.WgtDate);
            sw.WriteLine(r.WgtWhen.DayNumber.ToString());
            sw.WriteLine(r.WgtKilograms.ToString(CultureInfo.InvariantCulture));
        }
    }

    public void SaveAllData()
    {
        SaveBloodPressureData();
        // SaveWaistData();
        SaveWeightData();
        SaveHeightData();
    }

    public List<Tension> TensionReadings => _tensionReadings;
    // public List<Waist> WaistReadings => _waistReadings;
    public List<Weight> WeightReadings => _weightReadings;

    // public void EditWaistReading(int index, DateOnly newDate, double newCm)
    // {
    //     _waistReadings[index].WstWhen = newDate;
    //     _waistReadings[index].WstCentimetres = newCm;
    // }

    public void EditWeightReading(int index, DateOnly newDate, double newKg)
    {
        _weightReadings[index].WgtWhen = newDate;
        _weightReadings[index].WgtKilograms = newKg;
    }

    public void EditBloodPressureReading(int index, DateTime newDate, int newDiastolic, int newSystolic, int newPulse)
    {
        _tensionReadings[index].BprWhen = newDate;
        _tensionReadings[index].BpDiastolic = newDiastolic;
        _tensionReadings[index].BpSystolic = newSystolic;
        _tensionReadings[index].Pulse =newPulse;
    }

    // public void AddWaistReading(DateOnly newDate, double newCm)
    // {
    //     Waist n = new Waist
    //     {
    //         WstWhen = newDate, WstCentimetres = newCm
    //     };
    //     _waistReadings.Add(n);
    // }

    public void AddWeightReading(DateOnly newDate, double newKg)
    {
        Weight n = new Weight
        {
            WgtWhen = newDate, WgtKilograms = newKg
        };
        _weightReadings.Add(n);
    }

    public void AddBloodPressureReading(DateTime newDate, int newDiastolic, int newSystolic, int newPulse)
    {
        Tension t=new Tension()
        {
            BprWhen = newDate, BpDiastolic = newDiastolic, BpSystolic = newSystolic, Pulse = newPulse
        };
        _tensionReadings.Add(t);
    }

    //public DateOnly LastBloodPressureReadingDate => _bloodPressureReadings[^1].BprWhen;
    public DateOnly LastTensionReadingDate => DateOnly.FromDateTime( _tensionReadings[^1].BprWhen);

    // public DateOnly LastWaistReadingDate => _waistReadings[^1].WstWhen;

    public DateOnly LastWeightReadingDate => _weightReadings[^1].WgtWhen;

    public string LastBloodPressureReadingValue
    {
        get
        {
            // string s = "B.P. = " + _tensionReadings[^1].BpSystolic.ToString() + " / " +
            //            _tensionReadings[^1].BpDiastolic.ToString();
            // s += " Pulse = " + _tensionReadings[^1].Pulse.ToString();
            string t
                = $"B.P. = {_tensionReadings[^1].BpSystolic} / {_tensionReadings[^1].BpDiastolic} Pulse = {_tensionReadings[^1].Pulse}";
            return t;
        }
    }

    // public string LastWaistReadingValue =>
    //     _waistReadings[^1].WstCentimetres.ToString(CultureInfo.CurrentCulture) + " cm";

    public string LastWeightReadingImperialString =>
        BodyStatics.WeightAsStonesAndPoundsString(_weightReadings[^1].WgtKilograms);

    public double LastWeightReadingKg => _weightReadings[^1].WgtKilograms;

    public double LastWeightReadingBmi => BodyStatics.BmiOf(_weightReadings[^1].WgtKilograms, _heightInMetres);

    public double HeightInMetres
    {
        get => _heightInMetres;
        set => _heightInMetres = value;
    }

    public double IdealWeightLowerLimit => BodyStatics.IdealWeightLowerLimitKg(_heightInMetres);

    public double IdealWeightHigherLimit => BodyStatics.IdealWeightHigherLimit(_heightInMetres);
}