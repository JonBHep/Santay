using System;

namespace Santay;

public class PrevuInfo : IComparable<PrevuInfo>
{
    public string Description { get; set; }
    public string Notes { get; set; }

    public PrevuInfo()
    {
        Description = string.Empty;
        Notes = string.Empty;
    }

    public string Specification
    {
        get
        {
            var parts = new string[2];
            parts[0] =Description;
            parts[1] =Notes;
            return string.Join(PrevuDate.PropertyMarker, parts);
        }
        
        set
        {
            var parts = value.Split(PrevuDate.PropertyMarker.ToCharArray());
            
            Description = parts[0];
            Notes = parts[1];
        }
    }

    public int CompareTo(PrevuInfo? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(Description, other.Description, StringComparison.Ordinal);
    }
    
}