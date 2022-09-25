using System.Collections.Generic;

public class RandomTable
{
    private Xorshift Xorshift;
    private List<float> Table;
    
    public RandomTable(int length)
    {
        Xorshift = new Xorshift();
        Table = new List<float>();

        for (var i = 0; i < length; i++)
        {
            Table.Add((float)Xorshift.NextDouble());
        }
    }

    public float Get(int index)
    {
        return Table[index % Table.Count];
    }
}