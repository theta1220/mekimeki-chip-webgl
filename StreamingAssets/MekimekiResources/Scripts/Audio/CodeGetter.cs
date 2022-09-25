using System.Collections.Generic;
using System.Linq;

public static class CodeGetter
{
    public static Dictionary<string, int[]> Cache = new Dictionary<string, int[]>();

    public static int[] Get(string codeName)
    {
        if (Cache.ContainsKey(codeName)) return Cache[codeName];

        // デフォルトはメジャー
        int first = 0;
        int second = 4;
        int third = 7;
        int fourth = -1;

        if (codeName.Contains("m"))
        {
            second = 3;
        }

        if (codeName.Contains("sus4"))
        {
            second = 5;
        }

        if (codeName.Contains("add9"))
        {
            second = 2;
        }

        if (codeName.Contains("aug"))
        {
            third = 8;
        }

        if (codeName.Contains("5"))
        {
            third = 6;
        }

        if (codeName.Contains("6"))
        {
            fourth = 9;
        }

        if (codeName.Contains("7"))
        {
            fourth = 10;
        }

        if (codeName.Contains("M"))
        {
            fourth = 11;
        }

        if (codeName.Contains("dim"))
        {
            second = 3;
            third = 6;
            fourth = 9;
        }

        if (codeName.Contains("oct"))
        {
            second = 12;
        }

        var arr = new int[] { first, second, third, fourth };
        Cache.Add(codeName, arr);
        return arr;
    }
}