using System;

namespace Santay;

public class PlannerEntry : IComparable<PlannerEntry>
{
    
    private const string Marker = "^";
    public DateOnly EntryDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public bool AddedToOutlook { get; set; }
    public bool InfoOnly { get; set; }

    public PlannerEntry()
    {
        Description = string.Empty;
        Notes = string.Empty;
    }

    public string Specification
    {
        get
        {
            var parts = new string[7];
            parts[0] = $"{EntryDate.DayNumber}";
            parts[1] = $"{StartTime.Ticks}";
            parts[2] = $"{EndTime.Ticks}";
            parts[3] =Description;
            parts[4] =Notes;
            parts[5] = $"{AddedToOutlook}";
            parts[6] = $"{InfoOnly}";
            return string.Join(Marker, parts);
        }
        
        set
        {
            var parts = value.Split(Marker.ToCharArray());
            
            if (int.TryParse(parts[0], out var dt))
            {
                EntryDate =DateOnly.FromDayNumber(dt);
            }

            if (long.TryParse(parts[1], out var ls))
            {
                StartTime=new TimeOnly(ls);
            }
            
            if (long.TryParse(parts[2], out var le))
            {
                EndTime=new TimeOnly(le);
            }

            Description = parts[3];

            Notes = parts[4];
            
            if (bool.TryParse(parts[5], out var oLook))
            {
                AddedToOutlook=oLook;
            }
            
            if (bool.TryParse(parts[6], out var iOnly))
            {
                InfoOnly=iOnly;
            }
            
        }
    }

    public int CompareTo(PlannerEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var entryDateComparison = EntryDate.CompareTo(other.EntryDate);
        if (entryDateComparison != 0) return entryDateComparison;
        return StartTime.CompareTo(other.StartTime);
    }
    
}