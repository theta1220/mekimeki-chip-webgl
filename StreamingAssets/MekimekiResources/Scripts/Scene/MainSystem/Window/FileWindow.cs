using System;
using System.IO;
using Sirius.Engine;

public class FileWindow : WindowSystemBase
{
    public MainSystemScene MainSystemScene;
    public SoundEditMenuList MenuList;

    public FileWindow(MainSystemScene mainSystemScene) : base("file", 80, 110, true)
    {
        Window.Position.Set(5, 145);
        MainSystemScene = mainSystemScene;
        MenuList = new SoundEditMenuList();
        MenuList.AddButton("Export", () => Export(false));
        MenuList.AddButton("Export LC", () => Export(true));
        MenuList.AddButton("Load", Load);
        MenuList.AddButton("Save", Save);
        MenuList.Root.Set(0, 8);
        MenuList.Root.Parent = Window.InnerPosition;
    }

    public override void Update()
    {
        base.Update();
        MenuList.Update();
    }

    public override bool Draw()
    {
        if (!base.Draw())
        {
            return false;
        }
        MenuList.Draw();
        return true;
    }

    public void Save()
    {
        FileManager.Instance.SaveToJsonWithPanel(MainSystemScene.AudioMixer, "json");
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

        MainSystemScene.AudioMixer = mixer;
        foreach (var track in mixer.Tracks)
        {
            track.SetNextNote();
            track.SetPrevNote();
        }

        AudioUndoManager.Instance.SetAudioMixer(MainSystemScene.AudioMixer, MainSystemScene.WindowSystem.GridWindow.Grids);
        MainSystemScene.WindowSystem.RemakeGridWindow();
    }

    public void Export(bool isLoopMode)
    {
        WaveExporter.Export(MainSystemScene.AudioMixer, isLoopMode);
    }
}