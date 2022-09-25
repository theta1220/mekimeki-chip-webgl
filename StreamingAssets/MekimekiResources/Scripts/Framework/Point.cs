using System;
using System.Text.Json.Serialization;

[Serializable]
public class Point
{
    [JsonInclude] public int X;
    [JsonInclude] public int Y;

    public Point()
    {
        X = 0;
        Y = 0;
    }

    [JsonConstructor]
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point operator +(Point a, Point b)
    {
        var point = new Point();
        point.X = a.X + b.X;
        point.Y = a.Y + b.Y;
        return point;
    }

    public static Point operator -(Point a, Point b)
    {
        var point = new Point();
        point.X = a.X - b.X;
        point.Y = a.Y - b.Y;
        return point;
    }
}