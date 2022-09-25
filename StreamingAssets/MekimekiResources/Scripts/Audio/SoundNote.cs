using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Serializable]
public class SoundNote
{
    [JsonInclude] public ReferenceValue<int> BeatOffset;
    [JsonInclude] public ReferenceValue<int> Melody;
    [JsonInclude] public ReferenceValue<int> Length;
    [JsonInclude] public ReferenceValue<int> Add;
    [JsonInclude] public ReferenceValue<int> AddKey;
    [JsonInclude] public ReferenceValue<bool> Slide;
    [JsonInclude] public ReferenceValue<int> AttackWaveType;
    [JsonInclude] public ReferenceValue<int> OverrideWaveType;
    [JsonInclude] public WaveVolume WaveVolume;

    [JsonIgnore] public AudioChannel AudioChannel;
    [JsonIgnore] public SoundNote NextNote;
    [JsonIgnore] public SoundNote PrevNote;

    public readonly string[] Codes = new[] { "", "m", "m", "", "", "m", "m7-5" };

    [JsonConstructor]
    public SoundNote(
        ReferenceValue<int> beatOffset,
        ReferenceValue<int> melody,
        ReferenceValue<int> length,
        ReferenceValue<int> add,
        ReferenceValue<int> addKey,
        ReferenceValue<bool> slide,
        ReferenceValue<int> attackWaveType,
        ReferenceValue<int> overrideWaveType,
        WaveVolume waveVolume)
    {
        BeatOffset = beatOffset ?? new ReferenceValue<int>(0);
        Melody = melody ?? new ReferenceValue<int>(0);
        Length = length ?? new ReferenceValue<int>(1);
        Add = add ?? new ReferenceValue<int>(0);
        AddKey = addKey ?? new ReferenceValue<int>(0);
        Slide = slide ?? new ReferenceValue<bool>(false);
        AttackWaveType = attackWaveType ?? new ReferenceValue<int>((int)WaveType.None);
        OverrideWaveType = overrideWaveType ?? new ReferenceValue<int>((int)WaveType.None);
        WaveVolume = waveVolume;

        if (waveVolume == null)
        {
            throw new Exception("SoundNoteでWaveVolumeがない");
        }
    }

    public void Initialize(AudioChannelType audioChannelType)
    {
        AudioChannel = new AudioChannel(audioChannelType);
    }

    public float Calc(ulong sampleCount, int bpm, int key, VirtualTrack track, WaveType waveType)
    {
        if ((WaveType)OverrideWaveType.Value != WaveType.None)
        {
            waveType = (WaveType)OverrideWaveType.Value;
        }

        var sec = sampleCount / 2.0 / AudioPlayer.SampleRate;
        var beat = bpm * (sec / 60.0) * 4; // 16分音符基準なので/4している
        var noteTime = beat - BeatOffset.Value;

        if (AudioChannel.AudioChannelType == AudioChannelType.Code)
        {
            var buf = 0f;
            var code = CodeGetter.Get(Codes[Melody.Value % Codes.Length]);
            foreach (var melody in code)
            {
                if (melody < 0)
                {
                    continue;
                }

                buf += AudioChannel.CalcWave(
                           sampleCount, track, this, NextNote, bpm, key + melody, waveType, noteTime)
                       * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);
            }

            return buf;
        }
        else if (AudioChannel.AudioChannelType == AudioChannelType.Octave)
        {
            var buf = 0f;
            buf += AudioChannel.CalcWave(
                       sampleCount, track, this, NextNote, bpm, key, waveType, noteTime)
                   * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);
            buf += AudioChannel.CalcWave(
                       sampleCount, track, this, NextNote, bpm, key + 12, waveType, noteTime)
                   * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);

            return buf;
        }
        else if (AudioChannel.AudioChannelType == AudioChannelType.CodeArp)
        {
            var code = CodeGetter.Get(Codes[Melody.Value % Codes.Length]);
            var melody = code[(int)(beat * code.Length) % code.Length];
            return AudioChannel.CalcWave(
                       sampleCount, track, this, NextNote, bpm, key + melody, waveType, noteTime)
                   * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);
        }
        else if (AudioChannel.AudioChannelType == AudioChannelType.CodeArp2)
        {
            var code = CodeGetter.Get(Codes[Melody.Value % Codes.Length]);
            var pattern = new[]
            {
                code[0], code[1], code[2], code[1], code[0] + 12, code[1], code[2], code[1]
            };
            var melody = pattern[(int)(beat % pattern.Length)];
            return AudioChannel.CalcWave(
                       sampleCount, track, this, NextNote, bpm, key + melody, waveType, noteTime)
                   * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);
        }
        else
        {
            // アタック部分の波形をチェンジ
            if (WaveVolume.Adsr.IsAttackOrDecay(this, noteTime) &&
                AttackWaveType.Value != (int)WaveType.None)
            {
                waveType = (WaveType)AttackWaveType.Value;
            }

            return AudioChannel.CalcWave(
                       sampleCount, track, this, NextNote, bpm, key, waveType, noteTime)
                   * WaveVolume.Calc(sampleCount, track, this, noteTime, Length.Value);
        }
    }
}