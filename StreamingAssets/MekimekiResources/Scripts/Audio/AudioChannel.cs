using System;
using System.Text.Json.Serialization;
using Sirius.Engine;

[Serializable]
public class AudioChannel
{
    public double ScaleFreq = 1.059463094;
    public const double C0 = 32.70319566257483;
    private static readonly int[] Melodies = new[] { 0, 2, 4, 5, 7, 9, 11 };
    // private static readonly int[] Melodies = new[] { 0, 2, 3, 5, 7, 8, 10 };

    [JsonInclude] public HighPassFilter HighPassFilterR;
    [JsonInclude] public HighPassFilter HighPassFilterL;
    [JsonInclude] public LowPassFilter LowPassFilterR;
    [JsonInclude] public LowPassFilter LowPassFilterL;

    private WaveGenerator _waveGenerator;
    public AudioChannelType AudioChannelType;

    public ulong Count;
    public double CurrentR;

    public AudioChannel(AudioChannelType audioChannelType)
    {
        AudioChannelType = audioChannelType;
        HighPassFilterR = new HighPassFilter(10, 30000, AudioPlayer.SampleRate);
        HighPassFilterL = new HighPassFilter(10, 30000, AudioPlayer.SampleRate);
        LowPassFilterR = new LowPassFilter(19000, 10, AudioPlayer.SampleRate);
        LowPassFilterL = new LowPassFilter(19000, 10, AudioPlayer.SampleRate);
        _waveGenerator = new WaveGenerator();
        ScaleFreq = Math.Pow(2, 1.0 / 12);
    }

    public void PlayReady()
    {
        HighPassFilterR.PlayReady();
        HighPassFilterL.PlayReady();
        LowPassFilterL.PlayReady();
        LowPassFilterR.PlayReady();
        Count = 0;
        CurrentR = 0;
    }

    public float CalcWave(ulong sampleCount, VirtualTrack track, SoundNote note, SoundNote nextNote,
        int bpm, int key, WaveType waveType, double noteTime)
    {
        if (note == null || note.Melody.Value < 0)
        {
            return 0;
        }

        var r = GetR(track, note, key);
        var count = noteTime * (60.0 / bpm) / 4 * AudioPlayer.SampleRate;

        if (Count >= CurrentR)
        {
            // ビブラート
            if (noteTime >= 3 && !note.Slide.Value &&
                AudioChannelType != AudioChannelType.Chorus &&
                AudioChannelType != AudioChannelType.Code &&
                AudioChannelType != AudioChannelType.CodeArp &&
                AudioChannelType != AudioChannelType.CodeArp2)
            {
                r += Math.Sin(noteTime * 3.5) * 1.5;
            }

            // スライド
            if (note.Slide.Value && nextNote != null)
            {
                var nextR = GetR(track, nextNote, key);
                r += (nextR - r) * (noteTime / note.Length.Value);
                nextNote.AudioChannel.Count = Count;
            }

            // キック
            if (AudioChannelType == AudioChannelType.Kick)
            {
                var s = noteTime * noteTime * 500;
                r += s;
            }
            
            // 良いキック
            if (AudioChannelType == AudioChannelType.SuperKick)
            {
                var realTime = noteTime / bpm;
                var s = realTime * 150000;
                r *= 0.2;
                r += s;
            }

            // 高速アルペジオ
            if (AudioChannelType == AudioChannelType.Chorus)
            {
                if ((int)(noteTime * 2) % 2 == 0)
                {
                    r /= 2;
                }
            }

            CurrentR = r;
            Count = 0;
        }

        if (note.Slide.Value && note.NextNote != null)
        {
            note.NextNote.AudioChannel.Count = Count;
            note.NextNote.AudioChannel.CurrentR = CurrentR;
        }

        var buf = 0.0f;
        if (note.AudioChannel.AudioChannelType == AudioChannelType.Code ||
            note.AudioChannel.AudioChannelType == AudioChannelType.Octave)
        {
            buf = _waveGenerator.Generate(note.Melody.Value, r, count, waveType);
        }
        else
        {
            buf = _waveGenerator.Generate(note.Melody.Value, CurrentR, Count, waveType);
        }

        // ベル
        if (AudioChannelType == AudioChannelType.Bell)
        {
            var time = count % r / r * Math.PI;
            var moduration = 5;
            var repeat = 2;
            for (var i = 0; i < repeat; i++)
            {
                buf *= (float)Math.Sin(time * moduration);
            }
        }

        if (sampleCount % 2 == 0 || AudioPlayer.Instance.MonoralMode.Value)
        {
            Count++;
            if (note.PrevNote != null && note.PrevNote.Slide.Value)
            {
                buf = note.PrevNote.AudioChannel.HighPassFilterR.Calc(buf);
                return note.PrevNote.AudioChannel.LowPassFilterR.Calc(buf);
            }
            else
            {
                buf = LowPassFilterR.Calc(buf);
                return HighPassFilterR.Calc(buf);
            }
        }
        else
        {
            if (note.PrevNote != null && note.PrevNote.Slide.Value)
            {
                buf = note.PrevNote.AudioChannel.HighPassFilterL.Calc(buf);
                return note.PrevNote.AudioChannel.LowPassFilterL.Calc(buf);
            }
            else
            {
                buf = LowPassFilterL.Calc(buf);
                return HighPassFilterL.Calc(buf);
            }
        }
    }

    private double GetR(VirtualTrack track, SoundNote note, int key)
    {
        var pitch = track.Pitch.Value;
        var melody = GetMelody(note.Melody.Value, key, track.Octave.Value, note.Add.Value, note.AddKey.Value) + pitch;
        melody -= 69;
        var freq = 440 * Math.Pow(ScaleFreq, melody);
        var r = AudioPlayer.SampleRate / freq / 2;
        return r;
    }

    private double GetMelody(int noteMelody, int key, int octave, int add, int addKey)
    {
        var melody = Melodies[noteMelody % Melodies.Length]
                     + noteMelody / Melodies.Length * 12
                     + key + octave * 12
                     + add
                     + addKey;
        return melody;
    }
}