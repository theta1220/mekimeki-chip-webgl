using System;
using System.IO;
using System.Linq;
using Sirius.Engine;

public class SoundEditScene : SceneBase
{
    public AudioMixer AudioMixer;
    public Text PlayStatusText;
    public Text CalcTimeText;
    public Text UpdateTimeSpanText;
    public Text DrawTimeSpanText;
    public Position Root;

    public const string PlayText = "play";
    public const string StopText = "stop";

    public SoundEditScene()
    {
    }

    public override void OnEnter()
    {
        Root = new Position();
        AudioMixer = new AudioMixer(null, null, null, null, null);
        AudioMixer.Format(180, 0);

        PlayStatusText = new Text(2);
        PlayStatusText.Position.Set(0, 20);
        CalcTimeText = new Text(2);
        CalcTimeText.Position.Set(0, 30);
        UpdateTimeSpanText = new Text(2);
        UpdateTimeSpanText.Position.Set(0, 40);
        DrawTimeSpanText = new Text(2);
        DrawTimeSpanText.Position.Set(0, 50);

        var scoreEditState = new SoundEditScoreEditState(this);
        var settingState = new SoundEditSettingState(this);
        StateMachine.Register(scoreEditState);
        StateMachine.Register(settingState);
        AudioUndoManager.Instance.SetAudioMixer(AudioMixer, scoreEditState.Grids);
        StateMachine.Switch<SoundEditScoreEditState>();
    }

    public void Load()
    {
        AudioMixer mixer = null;
        var path = Sirius.Engine.Framework.FileUtil.Manager.LoadFilePanel("json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var json = File.ReadAllText(path);
        try
        {
            var loaded = System.Text.Json.JsonSerializer.Deserialize<AudioMixer>(json);
            Logger.Info("最新状態でロード");
            mixer = loaded;
        }
        catch
        {
            try
            {
                // 古い形式で読み込みをトライする
                var loaded = System.Text.Json.JsonSerializer.Deserialize<AudioMixerVersion0>(json);
                Logger.Info("version0でロード");
                mixer = new AudioMixer(loaded);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        foreach (var tra in mixer.Tracks)
        {
            if (tra.AudioChannelType.Value == (int)AudioChannelType.Code)
            {
                tra.Octave.Value = 2;
            }
        }

        AudioMixer = mixer;
        foreach (var track in mixer.Tracks)
        {
            track.SetNextNote();
            track.SetPrevNote();
        }
        var state = new SoundEditScoreEditState(this);
        AudioUndoManager.Instance.SetAudioMixer(AudioMixer, state.Grids);
        StateMachine.New(state);
        StateMachine.Current = state;
    }

    public override void Update()
    {
        base.Update();
        PlayStatusText.SetText(AudioPlayer.Instance.IsPlaying ? PlayText : StopText);
        CalcTimeText.SetText($"{AudioPlayer.Instance.CalcTimeSpan.TotalMilliseconds}");
        UpdateTimeSpanText.SetText($"{SceneManager.Instance.UpdateTimeSpan.TotalMilliseconds}");
        DrawTimeSpanText.SetText($"{SceneManager.Instance.DrawTimeSpan.TotalMilliseconds}");
    }

    public override void Draw()
    {
        base.Draw();
        PlayStatusText.Draw();
        // CalcTimeText.Draw();
        // UpdateTimeSpanText.Draw();
        // DrawTimeSpanText.Draw();
    }
}