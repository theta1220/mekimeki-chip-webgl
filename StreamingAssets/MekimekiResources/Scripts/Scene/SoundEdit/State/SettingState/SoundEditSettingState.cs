using Sirius.Engine;

public class SoundEditSettingState : StateBase
{
    public SoundEditSettingWindow SettingWindow;
    public SoundEditScene Scene;

    public SoundEditSettingState(SoundEditScene scene)
    {
        Scene = scene;
    }
    
    public override void OnEnter()
    {
        SettingWindow = new SoundEditSettingWindow(Scene.AudioMixer);
    }

    public override void Update()
    {
        SettingWindow.Update();

        if (Input.Instance.Enter.IsPushEnd)
        {
            Scene.StateMachine.Switch<SoundEditScoreEditState>();
        }
    }

    public override void Draw()
    {
        SettingWindow.Draw();
    }
}