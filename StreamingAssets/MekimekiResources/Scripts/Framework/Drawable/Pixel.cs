
using System;
using System.Text.Json.Serialization;

[Serializable]
public class Pixel
{
    [JsonInclude] public Point Point;
    [JsonInclude] public byte Color;

    public Pixel()
    {
        Point = new Point();
        Color = 0;
    }

    public Pixel(int x, int y, byte color)
    {
        Point = new Point()
        {
            X = x,
            Y = y,
        };
        Color = color;
    }
}