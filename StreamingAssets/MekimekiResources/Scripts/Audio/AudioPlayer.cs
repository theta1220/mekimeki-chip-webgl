using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sirius.Engine.Framework.Audio;

public class AudioPlayer : Singleton<AudioPlayer>, IAudio
{
    public const int SampleRate = 44100;
    public ulong Count;
    public ulong StackCount;
    public bool IsPlaying;
    public AudioMixer Current = null;
    public AudioMixer PreviewMixer;
    public SoundNote MidiKeyPreviewNote;
    public TimeSpan CalcTimeSpan;
    public Stopwatch Stopwatch;
    public ReferenceValue<bool> MonoralMode;
    public ReferenceValue<bool> LoopMode;
    public List<float[]> Buffers;
    public ulong AllSampleSize;

    public AudioPlayer()
    {
        CalcTimeSpan = new TimeSpan();
        Stopwatch = new Stopwatch();
        IsPlaying = false;
        AudioThread.Instance.SetAudio(this);
        MonoralMode = new ReferenceValue<bool>(false);
        LoopMode = new ReferenceValue<bool>(false);
        Buffers = new List<float[]>();
    }

    public void OnAudioRead(float[] data)
    {
        Stopwatch.Restart();
        
        if (!IsPlaying)
        {
            Current = null;

            if (MonoralMode.Value)
            {
                for (ulong i = 0; i < (ulong)data.Length / 2; i++)
                {
                    var buf = PreviewMixer?.Calc(Count + i * 2);
                    if (!buf.HasValue)
                    {
                        data[i * 2] = 0;
                        data[i * 2 + 1] = 0;
                    }
                    else
                    {
                        data[i * 2] = buf.Value;
                        data[i * 2 + 1] = buf.Value;
                    }
                }
            }
            else
            {
                for (var i = 0; i < data.Length; i++)
                {
                    var buf = PreviewMixer?.Calc(Count + (ulong)i);
                    if (!buf.HasValue)
                    {
                        data[i] = 0;
                    }
                    else
                    {
                        data[i] = buf.Value;
                    }
                }
            }

            Count += (ulong)data.Length;
            return;
        }

        if (MonoralMode.Value)
        {
            for (ulong i = 0; i < (ulong)data.Length / 2; i++)
            {
                var buf = Current?.Calc(Count);
                if (!buf.HasValue)
                {
                    data[i * 2] = 0;
                    data[i * 2 + 1] = 0;
                }
                else
                {
                    data[i * 2] = buf.Value;
                    data[i * 2 + 1] = buf.Value;
                }

                if (LoopMode.Value && Count + i * 2 >= AllSampleSize)
                {
                    Current?.PlayReady();
                    Count = 0;
                }
                else
                {
                    Count+=2;
                }
            }
        }
        else
        {
            for (ulong i = 0; i < (ulong)data.Length; i++)
            {
                var buf = Current?.Calc(Count);
                if (!buf.HasValue)
                {
                    data[i] = 0;
                }
                else
                {
                    data[i] = buf.Value;
                }

                if (LoopMode.Value && Count + i * 2 >= AllSampleSize)
                {
                    Current?.PlayReady();
                    Count = 0;
                }
                else
                {
                    Count++;
                }
            }
        }

        Stopwatch.Stop();
        CalcTimeSpan = Stopwatch.Elapsed;
        lock (Buffers)
        {
            Buffers.Add(data);
        }
    }

    public void PlayPreview(AudioMixer mixer)
    {
        mixer.PlayReady();
        Count = 0;
        PreviewMixer = mixer;
    }

    public void Play(ulong sampleCount, AudioMixer mixer)
    {
        PreviewMixer = null;
        mixer.PlayReady();
        Current = mixer;
        IsPlaying = true;
        Count = sampleCount;
        StackCount = Count;
        AllSampleSize = GetAllSampleSize();
    }

    public void Stop()
    {
        IsPlaying = false;

        while (Current != null)
        {
        }
        Count = 0;
    }

    public float[] GetBuffer()
    {
        lock (Buffers)
        {
            var buf = Buffers.SelectMany(_ => _).ToArray();
            return buf;
        }
    }

    public void RemoveBuffer()
    {
        lock (Buffers)
        {
            Buffers.Clear();
        }
    }

    public ulong GetAllSampleSize()
    {
        ulong lastBeat =
            (ulong)Current.Tracks.Max(_ =>
            {
                var lastNote = _.Notes.OrderByDescending(
                    note => note.BeatOffset.Value + note.Length.Value).FirstOrDefault();
                if (lastNote == null) return 0;
                return lastNote.BeatOffset.Value + lastNote.Length.Value;
            });
        
        var bpm = Current.Bpm.Value;
        double sec = bpm / 60.0;
        ulong sampleNum = (ulong)(AudioPlayer.SampleRate / sec) * 4 / 16 * 2;
        ulong allSampleNum = sampleNum * lastBeat;
        return allSampleNum;
    }
}