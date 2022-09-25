using System;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

[Serializable]
public class WaveAdsr
{
    [JsonInclude] public ReferenceValue<double> AttackTime;
    [JsonInclude] public ReferenceValue<double> DecayTime;
    [JsonInclude] public ReferenceValue<double> SustainLevel;
    [JsonInclude] public ReferenceValue<double> ReleaseTime;

    public WaveAdsr()
    {
        AttackTime = new ReferenceValue<double>(0);
        DecayTime = new ReferenceValue<double>(0);
        SustainLevel = new ReferenceValue<double>(0);
        ReleaseTime = new ReferenceValue<double>(0);
    }

    [JsonConstructor]
    public WaveAdsr(
        ReferenceValue<double> attackTime,
        ReferenceValue<double> decayTime,
        ReferenceValue<double> sustainLevel,
        ReferenceValue<double> releaseTime)
    {
        AttackTime = attackTime;
        DecayTime = decayTime;
        SustainLevel = sustainLevel;
        ReleaseTime = releaseTime;
    }

    public float Calc(SoundNote note, double noteTime, double noteLength)
    {
        // Attack
        if (noteTime < AttackTime.Value &&
            (note.PrevNote == null ||
             note.PrevNote != null && !note.PrevNote.Slide.Value))
        {
            var time = noteTime / AttackTime.Value;
            return (float)(time);
        }

        // Decay
        if (noteTime < DecayTime.Value &&
            (note.PrevNote == null ||
             note.PrevNote != null && !note.PrevNote.Slide.Value))
        {
            var time = (noteTime - AttackTime.Value) / (DecayTime.Value - AttackTime.Value);
            var level = (1 - SustainLevel.Value) * (1 - time) + SustainLevel.Value;
            return (float)level;
        }

        // Sustain
        if ((note.Length?.Value > 1 || note.Slide.Value) && noteTime < noteLength)
        {
            if (note.NextNote != null && note.Slide.Value)
            {
                var nextSustain = note.NextNote.WaveVolume.Adsr.SustainLevel.Value;
                var currentTime = noteTime / noteLength;
                var nextTime = (1 - currentTime);
                return (float)(SustainLevel.Value * nextTime + nextSustain * currentTime);
            }

            return (float)SustainLevel.Value;
        }

        // Release
        {
            var time = (noteTime - noteLength) / ReleaseTime.Value;
            var level = SustainLevel.Value * 0.5 * (1 - time);
            if (note.Length?.Value == 1)
            {
                level *= 2;
            }
            if (level < 0)
            {
                level = 0;
            }

            return (float)level;
        }
    }

    public bool IsAttackOrDecay(SoundNote note, double noteTime)
    {
        // Attack
        if (noteTime < AttackTime.Value &&
            (note.PrevNote == null ||
             note.PrevNote != null && !note.PrevNote.Slide.Value))
        {
            return true;
        }
        
        // Decay
        if (noteTime < DecayTime.Value &&
            (note.PrevNote == null ||
             note.PrevNote != null && !note.PrevNote.Slide.Value))
        {
            return true;
        }

        return false;
    }
}