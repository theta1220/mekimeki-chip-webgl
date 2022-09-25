using System;
using System.Collections.Generic;
using System.Net.Sockets;

public class SoundEditSettingMenu
{
    public Position Root;
    public MenuSelector MenuSelector;
    public AudioMixer AudioMixer;
    public ReferenceValue<int> EditingTrackIndex;

    public SoundEditSettingMenu(AudioMixer audioMixer, ReferenceValue<int> editingTrackIndex)
    {
        AudioMixer = audioMixer;
        EditingTrackIndex = editingTrackIndex;
        Root = new Position();
        Root.Set(-5, 0);
        MenuSelector = new MenuSelector(new List<IMenuOption>
        {
            new IntMenuOption(
                "BPM", AudioMixer.Bpm, 1, 999),
            new IntMenuOption(
                "KEY", AudioMixer.Key, -20, 20),
            new DoubleMenuOption(
                "LEVEL",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .Volume, 0, 1, 0.01),
            new DoubleMenuOption(
                "VELOCITY",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Volume, 0, 1, 0.01),
            new DoubleMenuOption(
                "ATTACK",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Adsr.AttackTime, 0, 1, 0.01),
            new DoubleMenuOption(
                "DECAY",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Adsr.DecayTime, 0, 1, 0.01),
            new DoubleMenuOption(
                "SUSTAIN",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Adsr.SustainLevel, 0, 1, 0.01),
            new DoubleMenuOption(
                "RELEASE",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Adsr.ReleaseTime, 0.1, 10, 0.1),
            new IntMenuOption(
                "WAV TYPE",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveType, 0, (int)WaveType.Max - 1, 1),
            new IntMenuOption(
                "ATK WAVE",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .AttackWaveType, -1, (int)WaveType.Max - 1, 1),
            new DoubleMenuOption(
                "PITCH",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .Pitch, 0, 1, 0.01),
            new BoolMenuOption(
                "MONO MODE",
                AudioPlayer.Instance.MonoralMode),
            new BoolMenuOption(
                "LOOP MODE",
                AudioPlayer.Instance.LoopMode),
            new DoubleMenuOption(
                "PAN",
                AudioMixer.Tracks[EditingTrackIndex.Value]
                    .WaveVolume.Pan, -1, 1, 0.01),
        });
        MenuSelector.Position.Parent = Root;
        MenuSelector.Controlable = false;
    }

    public void Update()
    {
        MenuSelector.Update();

        foreach (var option in MenuSelector.Options)
        {
            var track = AudioMixer.Tracks[EditingTrackIndex.Value];
            switch (option.Name)
            {
                case "BPM":
                    option.Reference = AudioMixer.Bpm;
                    break;
                case "KEY":
                    option.Reference = AudioMixer.Key;
                    break;
                case "LEVEL":
                    option.Reference = track.Volume;
                    break;
                case "VELOCITY":
                    option.Reference = track.WaveVolume.Volume;
                    break;
                case "ATTACK":
                    option.Reference = track.WaveVolume.Adsr.AttackTime;
                    break;
                case "DECAY":
                    option.Reference = track.WaveVolume.Adsr.DecayTime;
                    break;
                case "SUSTAIN":
                    option.Reference = track.WaveVolume.Adsr.SustainLevel;
                    break;
                case "RELEASE":
                    option.Reference = track.WaveVolume.Adsr.ReleaseTime;
                    break;
                case "WAVE TYPE":
                    option.Reference = track.WaveType;
                    break;
                case "ATK WAVE":
                    option.Reference = track.AttackWaveType;
                    break;
                case "PITCH":
                    option.Reference = track.Pitch;
                    break;
                case "MONO MODE":
                    option.Reference = AudioPlayer.Instance.MonoralMode;
                    break;
                case "LOOP MODE":
                    option.Reference = AudioPlayer.Instance.LoopMode;
                    break;
                case "PAN":
                    option.Reference = track.WaveVolume.Pan;
                    break;
            }
        }
    }

    public void UpdateBackground()
    {
        MenuSelector.UpdateBackground();
    }

    public void Draw()
    {
        MenuSelector.Draw();
    }
}