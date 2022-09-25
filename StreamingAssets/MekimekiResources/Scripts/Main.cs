using System;
using Sirius.Engine;
using Sirius.Engine.Analytics;

public class Main
{
    public Main()
    {
        SceneManager.Instance.Switch<MainSystemScene>();
        VirtualScreen.Instance.Initialize();
    }

    public void Update()
    {
        ProfilerUtil.Begin("app", "update");
        Sirius.Engine.Input.Instance.Update();
        SceneManager.Instance.Update();
        FrameCount.Update();
        ProfilerUtil.End();
    }

    public void Draw()
    {
        ProfilerUtil.Begin("app", "draw");
        SceneManager.Instance.Draw();
        VirtualScreen.Instance.Draw();
        Sirius.Engine.ScreenHandler.Draw();
        ProfilerUtil.End();
    }
}