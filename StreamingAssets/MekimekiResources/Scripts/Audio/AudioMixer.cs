using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms.VisualStyles;
using Cysharp.Threading.Tasks.Linq;
using NClone;

[Serializable]
public class AudioMixer
{
    public const int CurrentDataVersion = 2;
    [JsonInclude] public ReferenceValue<int> DataVersion;
    [JsonInclude] public ReferenceValue<int> Bpm;
    [JsonInclude] public ReferenceValue<int> Key;
    [JsonInclude] public List<VirtualTrack> Tracks;
    [JsonInclude] public ReferenceValue<double> MasterVolume;

    private float[] _phaseCache = new float[500]; 
    private int _phaseCacheCount = 0;

    [JsonConstructor]
    public AudioMixer(ReferenceValue<int> dataVersion, ReferenceValue<int> bpm,
        ReferenceValue<int> key, List<VirtualTrack> tracks, ReferenceValue<double> masterVolume)
    {
        DataVersion = dataVersion ?? new ReferenceValue<int>(CurrentDataVersion);
        Bpm = bpm ?? new ReferenceValue<int>(180);
        Key = key ?? new ReferenceValue<int>(0);
        Tracks = tracks ?? new List<VirtualTrack>();
        foreach (var track in Tracks)
        {
            track.AudioMixer = this;
        }

        MasterVolume = masterVolume ?? new ReferenceValue<double>(1);
    }

    public AudioMixer(AudioMixerVersion0 audioMixerVersion0)
    {
        DataVersion = new ReferenceValue<int>(CurrentDataVersion);
        Bpm = new ReferenceValue<int>(audioMixerVersion0.Bpm.Value);
        Key = new ReferenceValue<int>(audioMixerVersion0.Key.Value);
        MasterVolume = new ReferenceValue<double>(1);
        Tracks = new List<VirtualTrack>();
        foreach (var trackVersion3 in audioMixerVersion0.Tracks)
        {
            var notes = new List<SoundNote>();
            foreach (var note in trackVersion3.Notes)
            {
                var newNote = new SoundNote(
                    new ReferenceValue<int>(note.BeatOffset),
                    new ReferenceValue<int>(note.Melody),
                    new ReferenceValue<int>(note.Length),
                    new ReferenceValue<int>(note.Add),
                    new ReferenceValue<int>(0),
                    new ReferenceValue<bool>(note.Slide),
                    new ReferenceValue<int>((int)WaveType.None),
                    new ReferenceValue<int>((int)WaveType.None),
                    new WaveVolume(new WaveAdsr(
                            new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.AttackTime),
                            new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.DecayTime),
                            new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.SustainLevel),
                            new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.ReleaseTime)),
                        new ReferenceValue<double>(trackVersion3.WaveVolume.Volume),
                        new ReferenceValue<double>(0)));
                notes.Add(newNote);
            }

            var track = new VirtualTrack(
                new ReferenceValue<int>(trackVersion3.Octave),
                new ReferenceValue<int>((int)trackVersion3.WaveType),
                new ReferenceValue<int>((int)WaveType.None),
                new WaveVolume(
                    new WaveAdsr(
                        new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.AttackTime),
                        new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.DecayTime),
                        new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.SustainLevel),
                        new ReferenceValue<double>(trackVersion3.WaveVolume.Adsr.ReleaseTime)),
                    new ReferenceValue<double>(trackVersion3.WaveVolume.Volume),
                    new ReferenceValue<double>(0)),
                new ReferenceValue<int>((int)trackVersion3.AudioChannelType),
                new ReferenceValue<double>(trackVersion3.Volume),
                new ReferenceValue<double>(trackVersion3.Pitch),
                new ReferenceValue<bool>(false),
                new ReferenceValue<bool>(false),
                notes);
            track.AudioMixer = this;
            Tracks.Add(track);
        }

        foreach (var track in Tracks)
        {
            track.AudioMixer = this;
        }
    }

    public void Format(int bpm, int key)
    {
        Bpm = new ReferenceValue<int>(bpm);
        Key = new ReferenceValue<int>(key);
        Tracks = new List<VirtualTrack>()
        {
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(
                    new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(8)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.30, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(
                    new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(8)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.2, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(
                    new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(8)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.19, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square50, WaveType.None,
                new WaveVolume(
                    new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(3)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.Chorus, 0.05, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Saw, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(3)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(20)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.CodeArp, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(20)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.Code, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square25, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(20)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.CodeArp2, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Sin, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(10)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.Bell, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square50, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(3)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.Kick, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.Square50, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(3)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.SuperKick, 0.1, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.PinkNoise, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(1)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.07, 0, new List<SoundNote>()),
            new VirtualTrack(
                1, WaveType.WhiteNoise, WaveType.None,
                new WaveVolume(new WaveAdsr(
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(0),
                        new ReferenceValue<double>(1),
                        new ReferenceValue<double>(0.5)),
                    new ReferenceValue<double>(1),
                    new ReferenceValue<double>(0)),
                AudioChannelType.None, 0.06, 0, new List<SoundNote>()),
        };

        foreach (var track in Tracks)
        {
            track.AudioMixer = this;
        }
    }

    public void PlayReady()
    {
        foreach (var track in Tracks)
        {
            track.PlayReady();
        }

        for (var i = 0; i < _phaseCache.Length; i++)
        {
            _phaseCache[i] = 0;
        }
    }

    public float Calc(ulong sampleCount)
    {
        long count = (long)sampleCount;

        var buffer = 0.0f;
        foreach (var track in Tracks)
        {
            buffer += track.Calc(Bpm.Value, Key.Value, (ulong)sampleCount);
        }

        var buf = buffer * (float)MasterVolume.Value;
        // if (sampleCount % 2 == 1)
        // {
        //     _phaseCache[_phaseCacheCount] = buf;
        //     _phaseCacheCount++;
        //     if (_phaseCacheCount >= _phaseCache.Length)
        //     {
        //         _phaseCacheCount = 0;
        //     }
        //
        //     return _phaseCache[(_phaseCacheCount + _phaseCache.Length / 2) % _phaseCache.Length];
        // }

        return buf;
    }
}