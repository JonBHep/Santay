using System;

namespace Santay;

public class PrevuAction : IComparable<PrevuAction>
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public bool AddedToOutlook { get; set; }
    
    public PrevuAction()
    {
        Description = string.Empty;
        Notes = string.Empty;
    }

    public string Specification
    {
        get
        {
            var parts = new string[5];
            parts[0] = $"{StartTime.Ticks}";
            parts[1] = $"{EndTime.Ticks}";
            parts[2] =Description;
            parts[3] =Notes;
            parts[4] = $"{AddedToOutlook}";
            return string.Join(PrevuDate.PropertyMarker, parts);
        }
        
        set
        {
            var parts = value.Split(PrevuDate.PropertyMarker.ToCharArray());
            
            if (long.TryParse(parts[0], out var ls))
            {
                StartTime=new TimeOnly(ls);
            }
            
            if (long.TryParse(parts[1], out var le))
            {
                EndTime=new TimeOnly(le);
            }

            Description = parts[2];

            Notes = parts[3];
            
            if (bool.TryParse(parts[4], out var oLook))
            {
                AddedToOutlook=oLook;
            }
            
        }
    }

    public int CompareTo(PrevuAction? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return StartTime.CompareTo(other.StartTime);
    }
    
}