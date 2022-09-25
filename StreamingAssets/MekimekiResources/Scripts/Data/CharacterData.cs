using System;

[Serializable]
public class CharacterData
{
    public Guid Id { get; set; }
    public int ResourceId { get; set; }
    public int PlacedDirection { get; set; }
    public int ActionEventId { get; set; }
    public int HitEventId { get; set; }
    public int X  { get; set; }
    public int Y { get; set; }

    public CharacterData()
    {
        Id = Guid.NewGuid();
    }
}