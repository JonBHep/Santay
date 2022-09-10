using System;
using System.Collections.Generic;

namespace Santay;

public class PrevuDate
{

    public PrevuDate()
    {
        Actions = new List<PrevuAction>();
        Infos = new List<PrevuInfo>();
    }
    
    public PrevuDate(PrevuDate mirror)
    {
        Actions = new List<PrevuAction>();
        Infos = new List<PrevuInfo>();
        Specification = mirror.Specification;
    }
    
    private const string SectionMarker = "^";
    private const string ItemMarker = "~";
    
    public const string PropertyMarker = "$";
    
    public List<PrevuAction> Actions;
    
    public List<PrevuInfo> Infos;
    public DateOnly EntryDate { get; set; }
    
    public string Specification
    {
        get
        {
            var parts = new string[5];
            parts[0] = $"{EntryDate.DayNumber}";
            parts[1] =ActionSpecifications;
            parts[2] =InfoSpecifications;
            return string.Join(SectionMarker, parts);
        }
        
        set
        {
            var parts = value.Split(SectionMarker.ToCharArray());
            
            if (int.TryParse(parts[0], out var ls))
            {
                EntryDate=DateOnly.FromDayNumber(ls);
            }

            ActionSpecifications = parts[1];
            InfoSpecifications = parts[2];
        }
    }

    private string ActionSpecifications
    {
        get
        {
            var outString=string.Empty;
            Actions.Sort();
            foreach (var prevuAction in Actions)
            {
                outString += $"{ItemMarker}{prevuAction.Specification}";
            }
            if (outString.Length > 0) outString = outString[1..];
            return outString;
        }
        set
        {
            var parts = value.Split(PrevuDate.ItemMarker.ToCharArray());
            Actions.Clear();
            foreach (var part in parts)
            {
                Actions.Add(new PrevuAction(){Specification = part});
            }
        }
    }
    
    private string InfoSpecifications
    {
        get
        {
            var outString=string.Empty;
            Infos.Sort();
            foreach (var prevuInfo in Infos)
            {
                outString += $"{ItemMarker}{prevuInfo.Specification}";
            }
            if (outString.Length > 0) outString = outString[1..];
            return outString;
        }
        set
        {
            var parts = value.Split(PrevuDate.ItemMarker.ToCharArray());
            Infos.Clear();
            foreach (var part in parts)
            {
                Infos.Add(new PrevuInfo(){Specification = part});
            }
        }
    }

    public bool InfoOnly => Infos.Count > 0 && Actions.Count < 1;
    public bool Empty => Infos.Count <1 && Actions.Count < 1;
    
}