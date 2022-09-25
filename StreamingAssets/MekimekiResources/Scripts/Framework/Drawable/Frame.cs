using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class Frame
{
    [JsonInclude] public List<Pixel> Pixels;
    [JsonIgnore] public Pixel[] PixelsCache;

    public Frame()
    {
        Pixels = new List<Pixel>();
    }

    public void Cache()
    {
        PixelsCache = Pixels.ToArray();
    }
}