using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

[Serializable]
public class Image
{
    [JsonInclude] public List<Frame> Frames;
    [JsonInclude] public ReferenceValue<int> AnimationInterval;

    [JsonIgnore] public Frame[] FramesCache;

    public Image()
    {
        Frames = new List<Frame>();
        AnimationInterval = new ReferenceValue<int>(0);
        AnimationInterval.Value = 6;
    }

    public void Cache()
    {
        FramesCache = Frames.ToArray();
        foreach (var frame in FramesCache)
        {
            frame.Cache();
        }
    }
}