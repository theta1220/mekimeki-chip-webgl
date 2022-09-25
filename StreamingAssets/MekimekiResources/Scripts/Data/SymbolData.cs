using System;

[Serializable]
public class SymbolData
{
    public Guid Id { get; set; }
    public int ResourceId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ActionEventId { get; set; }
    public int HitEventId { get; set; }

    public SymbolData()
    {
        Id = Guid.NewGuid();
    }
}