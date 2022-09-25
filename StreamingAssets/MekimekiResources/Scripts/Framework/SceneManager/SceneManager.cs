using System;
using System.Diagnostics;
using Sirius.Engine;

public class SceneManager : Singleton<SceneManager>
{
    public SceneBase Current;
    public TimeSpan UpdateTimeSpan;
    public Stopwatch UpdateStopwatch;
    public TimeSpan DrawTimeSpan;
    public Stopwatch DrawStopwatch;

    public SceneManager()
    {
        UpdateStopwatch = new Stopwatch();
        UpdateTimeSpan = new TimeSpan();
        DrawStopwatch = new Stopwatch();
        DrawTimeSpan = new TimeSpan();
    }

    public void Switch<T>() where T : SceneBase, new()
    {
        var next = new T();
        Current = next;
        Current.OnEnter();
    }

    public void Update()
    {
        UpdateStopwatch.Restart();
        Current?.Update();
        UpdateStopwatch.Stop();
        UpdateTimeSpan = UpdateStopwatch.Elapsed;
    }

    public void Draw()
    {
        DrawStopwatch.Restart();
        Current?.Draw();
        DrawStopwatch.Stop();
        DrawTimeSpan = DrawStopwatch.Elapsed;
    }
}