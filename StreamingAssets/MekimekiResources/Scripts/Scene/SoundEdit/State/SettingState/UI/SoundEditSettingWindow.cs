using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sirius.Engine;

public class SoundEditSettingWindow
{
    public const int Width = 500;
    public const int Height = 212;

    public Position Root;
    public List<SoundEditSettingKnob> Knobs;

    public SoundEditSettingWindow(AudioMixer audioMixer)
    {
        Root = new Position();
        Root.Set(20, 0);

        Knobs = new List<SoundEditSettingKnob>();
        var count = 0;
        var offset = (double)Width / (audioMixer.Tracks.Count + 2);
        var x = offset * count;
        foreach (var track in audioMixer.Tracks)
        {
            x = offset * count;
            var knob = new SoundEditSettingKnob($"CH{count}",
                track.Volume, track.Mute, track.Solo);
            knob.Root.Set((int)x, 20);
            knob.Root.Parent = Root;
            Knobs.Add(knob);
            count++;
        }
        x = offset * count;
        var master = new SoundEditSettingKnob("MAS",
            audioMixer.MasterVolume, null, null);
        master.Root.Set((int)x, 20);
        master.Root.Parent = Root;
        Knobs.Add(master);
    }

    public void Update()
    {
        foreach (var knob in Knobs)
        {
            knob.Update();
        }
    }

    public void Draw()
    {
        foreach (var knob in Knobs)
        {
            knob.Draw();
        }
    }
}