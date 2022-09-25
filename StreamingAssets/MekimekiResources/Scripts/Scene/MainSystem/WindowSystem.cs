using System;
using System.Collections.Generic;
using System.Linq;
using Sirius.Engine;

public class WindowSystem
{
    public Sprite BackGround;
    public List<WindowSystemBase> WindowSystems;
    public WindowSystemBase TargetWindow;
    public MouseCursor MouseCursor;
    public SystemDockBar SystemDockBar;
    public Text VersionText;
    public GridWindow GridWindow;
    public SoundMakeWindow SoundMakeWindow;
    public MainSystemScene MainSystemScene;
    public VolumeWindow VolumeWindow;

    public WindowSystem(MainSystemScene mainSystemScene, AudioMixer audioMixer)
    {
        MainSystemScene = mainSystemScene;
        GridWindow = new GridWindow(MainSystemScene.AudioMixer);
        SoundMakeWindow = new SoundMakeWindow(MainSystemScene.AudioMixer, GridWindow.EditingTrackIndex);
        VolumeWindow = new VolumeWindow(MainSystemScene.AudioMixer);
        RemakeGridWindow();
        WindowSystems = new List<WindowSystemBase>()
        {
            VolumeWindow,
            new FileWindow(mainSystemScene),
            new WaveMonitorWindow(),
            new ApplicationInfoWindow(),
            SoundMakeWindow,
            GridWindow,
        };
        TargetWindow = GridWindow;
        GridWindow.DrawPriority += 100;
        MouseCursor = new MouseCursor();
        SystemDockBar = new SystemDockBar(WindowSystems);
        VersionText = new Text(2);
        VersionText.SetText("mekimeki chip ver 2022-09-25.1");
        VersionText.Position.Parent = SystemDockBar.Position;
        VersionText.Position.Set(Screen.Width - (VersionText.SourceText.Length * 7 + 5), 7);

        BackGround = new Sprite("UI/main_background.png", false);
        WindowSystems.Sort((a, b) => a.DrawPriority - b.DrawPriority);
    }

    public void RemakeGridWindow()
    {
        GridWindow.Initialize(MainSystemScene.AudioMixer);
        SoundMakeWindow.Initialize(MainSystemScene.AudioMixer, GridWindow.EditingTrackIndex);
        VolumeWindow.Initialize(MainSystemScene.AudioMixer);
        GridWindow.OnTrackChangedEvents.Add(() =>
            SoundMakeWindow.Initialize(MainSystemScene.AudioMixer, GridWindow.EditingTrackIndex));
    }

    public void Update()
    {
        if (Input.Instance.MouseLeft.IsPushStartPure)
        {
            if (!SystemDockBar.BarSprite.IsHitCursor())
            {
                foreach (var windowSystem in WindowSystems.ToArray().Reverse())
                {
                    if (windowSystem.Window.IsHitCursor() && windowSystem.Window.Visible)
                    {
                        windowSystem.DrawPriority += 100;
                        windowSystem.Window.Visible = true;
                        break;
                    }
                }
            }

            var targetId = SystemDockBar.GetTargetWindowId();
            if (targetId != Guid.Empty)
            {
                var window = WindowSystems.First(_ => _.Id == targetId);

                if (TargetWindow == window && window.Window.Visible)
                {
                    window.Window.Visible = false;
                    window.DrawPriority -= 100;
                }
                else
                {
                    window.DrawPriority += 100;
                    window.Window.Visible = true;
                }
            }

            WindowSystems.Sort((a, b) => a.DrawPriority - b.DrawPriority);
            var priorityCount = 0;
            foreach (var windowSystem in WindowSystems)
            {
                windowSystem.DrawPriority = priorityCount;
                priorityCount++;
            }
        }

        WindowSystems.Sort((a, b) => a.DrawPriority - b.DrawPriority);
        TargetWindow = WindowSystems.OrderByDescending(_ => _.DrawPriority).First();
        if (TargetWindow == GridWindow)
        {
            VolumeWindow.Window.Visible = false;
        }
        else if (TargetWindow == VolumeWindow)
        {
            GridWindow.Window.Visible = false;
        }

        if (TargetWindow.Window.Visible && !SystemDockBar.BarSprite.IsHitCursor() && !TargetWindow.MoveWindow() &&
            TargetWindow.Window.IsHitCursor())
        {
            TargetWindow.Update();
        }

        foreach (var windowSystem in WindowSystems)
        {
            windowSystem.UpdateBackground();
        }

        SystemDockBar.SetActiveColor(TargetWindow.Id);

        MouseCursor.Update();
    }

    public void Draw()
    {
        foreach (var windowSystem in WindowSystems)
        {
            windowSystem.Draw();
        }

        SystemDockBar.Draw();
        VersionText.Draw();
        MouseCursor.Draw();
    }
}