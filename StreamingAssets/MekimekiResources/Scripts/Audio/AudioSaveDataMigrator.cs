using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class AudioMixerVersion0
{
    [JsonInclude] public ReferenceInt Bpm;
    [JsonInclude] public ReferenceInt Key;
    [JsonInclude] public List<VirtualTrackVersion0> Tracks;
}

[Serializable]
public class VirtualTrackVersion0
{
    [JsonInclude] public int Channel;
    [JsonInclude] public int Priority;
    [JsonInclude] public int Octave;
    [JsonInclude] public WaveType WaveType;
    [JsonInclude] public WaveVolumeVersion0 WaveVolume;
    [JsonInclude] public AudioChannelType AudioChannelType;
    [JsonInclude] public double Volume = 1;
    [JsonInclude] public double Pitch = 0;
    [JsonInclude] public List<SoundNoteVersion0> Notes;
}

[Serializable]
public class SoundNoteVersion0
{
    [JsonInclude] public int BeatOffset;
    [JsonInclude] public int Melody;
    [JsonInclude] public int Length;
    [JsonInclude] public int Add;
    [JsonInclude] public bool Slide;
}

[Serializable]
public class WaveVolumeVersion0
{
    [JsonInclude] public WaveAdsrVersion0 Adsr;
    [JsonInclude] public double Volume;
}

[Serializable]
public class WaveAdsrVersion0
{
    [JsonInclude] public double AttackTime;
    [JsonInclude] public double DecayTime;
    [JsonInclude] public double SustainLevel;
    [JsonInclude] public double ReleaseTime;
}