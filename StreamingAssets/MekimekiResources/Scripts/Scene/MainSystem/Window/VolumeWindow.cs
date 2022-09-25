    public class VolumeWindow : WindowSystemBase
    {
        public SoundEditSettingWindow SettingWindow;
        
        public VolumeWindow(AudioMixer audioMixer) : base("volume", 480, 200, true)
        {
            Initialize(audioMixer);
        }

        public void Initialize(AudioMixer audioMixer)
        {
            SettingWindow = new SoundEditSettingWindow(audioMixer);
            SettingWindow.Root.Parent = Window.InnerPosition;
            Window.Position.Set(10, 30);
        }

        public override void Update()
        {
            base.Update();
            SettingWindow.Update();
        }

        public override bool Draw()
        {
            if (!base.Draw())
            {
                return false;
            }
            SettingWindow.Draw();
            return false;
        }
    }
