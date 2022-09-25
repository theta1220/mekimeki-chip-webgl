public class MainSystemScene : SceneBase
{
    public WindowSystem WindowSystem;
    public AudioMixer AudioMixer;
    
    public override void OnEnter()
    {
        AudioMixer = new AudioMixer(null, null, null, null, null);
        AudioMixer.Format(180, 0);
        WindowSystem = new WindowSystem(this, AudioMixer);
    }

    public override void Update()
    {
        WindowSystem.Update();
    }

    public override void Draw()
    {
        WindowSystem.Draw();
    }
}