    public class SoundMakeWindow : WindowSystemBase
    {
        public SoundEditSettingMenu Menu;
        
        public SoundMakeWindow(AudioMixer audioMixer, ReferenceValue<int> editingTrackIndex) : base("sound", 128, 190, true)
        {
            Initialize(audioMixer, editingTrackIndex);
        }
        
        public void Initialize(AudioMixer audioMixer, ReferenceValue<int> editingTrackIndex)
        {
            Menu = new SoundEditSettingMenu(audioMixer, editingTrackIndex);
            Menu.Root.Parent = Window.InnerPosition;
            Window.Position.Set(89, 140);
        }

        public override void Update()
        {
            base.Update();
            Menu.Update();
        }

        public override void UpdateBackground()
        {
            base.UpdateBackground();
            Menu.UpdateBackground();
        }

        public override bool Draw()
        {
            if (!base.Draw())
            {
                return false;
            }
            Menu.Draw();
            return false;
        }
    }
