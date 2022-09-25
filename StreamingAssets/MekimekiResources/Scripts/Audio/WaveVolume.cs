
using System;
using System.Text.Json.Serialization;

[Serializable]
public class WaveVolume
{
    [JsonInclude] public WaveAdsr Adsr;
    [JsonInclude] public ReferenceValue<double> Volume;
    [JsonInclude] public ReferenceValue<double> Pan;

    [JsonConstructor]
    public WaveVolume(WaveAdsr adsr, ReferenceValue<double> volume, ReferenceValue<double> pan)
    {
        if (adsr == null)
        {
            throw new Exception("データ不整合です");
        }

        Adsr = adsr;
        Volume = volume ?? new ReferenceValue<double>(0);
        Pan = pan ?? new ReferenceValue<double>(0);
    }

    public float Calc(ulong sampleCount, VirtualTrack track, SoundNote note, double noteTime, double noteLength)
    {
        // R
        var pan = 1.0;
        if (sampleCount % 2 == 0)
        {
            if ((int)Math.Round(Pan.Value * 100) == 0)
            {
                pan += track.WaveVolume.Pan.Value;
            }
            else
            {
                pan += Pan.Value;
            }
        }
        // L
        else
        {
            if ((int)Math.Round(Pan.Value * 100) == 0)
            {
                pan -= track.WaveVolume.Pan.Value;
            }
            else
            {
                pan -= Pan.Value;
            }
        }
        return (float)(Adsr.Calc(note, noteTime, noteLength) * Volume.Value * pan);
    }
}