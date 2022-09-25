using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using NUnit.Framework.Constraints;
using Sirius.Engine;

[Serializable]
public class VirtualTrack
{
    [JsonInclude] public ReferenceValue<int> Octave;
    [JsonInclude] public ReferenceValue<int> WaveType;
    [JsonInclude] public ReferenceValue<int> AttackWaveType;
    [JsonInclude] public WaveVolume WaveVolume;
    [JsonInclude] public ReferenceValue<int> AudioChannelType;
    [JsonInclude] public ReferenceValue<double> Volume;
    [JsonInclude] public ReferenceValue<double> Pitch;
    [JsonInclude] public ReferenceValue<bool> Mute;
    [JsonInclude] public ReferenceValue<bool> Solo;
    [JsonInclude] public List<SoundNote> Notes;

    [JsonIgnore] public AudioMixer AudioMixer;

    private SoundNote _noteCache;
    private int _beatCache;
    private bool _lastNote;
    private bool _mute;
    private bool _solo;

    [JsonConstructor]
    public VirtualTrack(ReferenceValue<int> octave,
        ReferenceValue<int> waveType, ReferenceValue<int> attackWaveType,
        WaveVolume waveVolume, ReferenceValue<int> audioChannelType, ReferenceValue<double> volume,
        ReferenceValue<double> pitch, ReferenceValue<bool> mute, ReferenceValue<bool> solo,
        List<SoundNote> notes)
    {
        if (waveVolume == null)
        {
            throw new Exception("WaveVolumeが指定されていません");
        }

        Octave = octave ?? new ReferenceValue<int>(1);
        WaveType = waveType ?? new ReferenceValue<int>(0);
        AttackWaveType = attackWaveType ?? new ReferenceValue<int>(-1);
        WaveVolume = waveVolume;
        AudioChannelType = audioChannelType ?? new ReferenceValue<int>(0);
        Volume = volume ?? new ReferenceValue<double>(1);
        Pitch = pitch ?? new ReferenceValue<double>(0);
        Notes = notes;
        Mute = mute ?? new ReferenceValue<bool>(false);
        Solo = solo ?? new ReferenceValue<bool>(false);
        _mute = false;
        _solo = false;

        foreach (var note in Notes)
        {
            note.Initialize((AudioChannelType)AudioChannelType.Value);
        }
    }

    // 初期データ初期化用
    public VirtualTrack(int octave, WaveType waveType, WaveType attackWaveType,
        WaveVolume waveVolume, AudioChannelType audioChannelType, double volume, double pitch,
        List<SoundNote> notes)
    {
        Octave = new ReferenceValue<int>(octave);
        WaveType = new ReferenceValue<int>((int)waveType);
        AttackWaveType = new ReferenceValue<int>((int)attackWaveType);
        WaveVolume = waveVolume;
        AudioChannelType = new ReferenceValue<int>((int)audioChannelType);
        Volume = new ReferenceValue<double>(volume);
        Pitch = new ReferenceValue<double>(pitch);
        Notes = notes;
        Mute = new ReferenceValue<bool>(false);
        Solo = new ReferenceValue<bool>(false);
        _mute = false;
        _solo = false;

        foreach (var note in Notes)
        {
            note.Initialize((AudioChannelType)AudioChannelType.Value);
        }
    }

    public void PlayReady()
    {
        foreach (var note in Notes)
        {
            note.AudioChannel.PlayReady();
        }

        _noteCache = null;
        _beatCache = -1;
        _lastNote = false;
    }

    public SoundNote GetCurrentNote(int bpm, ulong sampleCount)
    {
        if (_lastNote)
        {
            return _noteCache;
        }

        var sec = sampleCount / 2.0 / AudioPlayer.SampleRate;
        var beat = (int)(bpm * (sec / 60.0) * 4); // 16分音符基準なので/4している
        if (_beatCache == beat)
        {
            if (_noteCache != null)
            {
                return _noteCache;
            }
            else
            {
                return null;
            }
        }

        if (_noteCache != null && _beatCache > beat && _beatCache + _noteCache.Length.Value <= beat)
        {
            return _noteCache;
        }

        _beatCache = beat;
        for (var i = 0; i < Notes.Count; i++)
        {
            if (Notes[i].BeatOffset.Value <= beat &&
                Notes[i].BeatOffset.Value + Notes[i].Length.Value
                                          + Notes[i].WaveVolume.Adsr.ReleaseTime.Value >= beat)
            {
                _noteCache = Notes[i];
            }
        }

        if (_noteCache == GetLastNote())
        {
            _lastNote = true;
        }

        return _noteCache;
    }

    public SoundNote GetLastNote()
    {
        if (Notes.Count == 0) return null;
        return Notes[Notes.Count - 1];
    }

    public List<SoundNote> RemoveNote(int beat, int melody)
    {
        var result = new List<SoundNote>();
        var notes = GetNote(beat, melody);
        foreach (var note in notes)
        {
            result.Add(note);
            Notes.Remove(note);
        }

        SetNextNote();
        SetPrevNote();
        return result;
    }

    public void RemoveNote(SoundNote note)
    {
        Notes.Remove(note);
        SetNextNote();
        SetPrevNote();
    }

    public List<SoundNote> GetNote(int beat, int melody)
    {
        return Notes
            .Where(_ => { return _.BeatOffset.Value == beat && _.Melody.Value == melody; })
            .ToList();
    }

    public void AddNote(SoundNote note)
    {
        if (Notes.Find(_ => _.BeatOffset == note.BeatOffset && _.Melody == note.Melody) != null)
        {
            return;
        }

        note.Initialize((AudioChannelType)AudioChannelType.Value);
        Notes.Add(note);
        Notes.Sort((a, b) => a.BeatOffset.Value - b.BeatOffset.Value);

        SetNextNote();
        SetPrevNote();

        _lastNote = false;
    }

    public void SetNextNote()
    {
        foreach (var note in Notes)
        {
            var nextIndex = Notes.IndexOf(note) + 1;
            if (nextIndex < Notes.Count)
            {
                note.NextNote = Notes[nextIndex];
            }
            else
            {
                note.NextNote = null;
            }
        }
    }

    public void SetPrevNote()
    {
        foreach (var note in Notes)
        {
            var prevIndex = Notes.IndexOf(note) - 1;
            if (prevIndex >= 0)
            {
                note.PrevNote = Notes[prevIndex];
            }
            else
            {
                note.PrevNote = null;
            }
        }
    }

    public float Calc(int bpm, int key, ulong sampleCount)
    {
        if (sampleCount % (ulong)(AudioPlayer.SampleRate * 0.5) == 0)
        {
            _mute = Mute.Value;

            var isSolo = false;
            foreach (var track in AudioMixer.Tracks)
            {
                if (track == this)
                {
                    continue;
                }

                if (track.Solo.Value)
                {
                    isSolo = true;
                    _solo = true;
                }
            }

            if (!isSolo)
            {
                _solo = false;
            }
        }

        if (_mute || _solo)
        {
            return 0f;
        }

        var buffer = 0f;
        var note = GetCurrentNote(bpm, sampleCount);
        if (note != null)
        {
            buffer = note.Calc(sampleCount, bpm, key, this, (WaveType)WaveType.Value) * (float)Volume.Value;
        }

        return buffer;
    }
}