    using System;

    public class SystemDockButton
    {
        public Position Position;
        public const int ButtonWidth = 50;
        public SliceSprite ButtonSprite;
        public Text TitleText;
        public Guid WindowId;

        public SystemDockButton(Guid windowId, string title)
        {
            WindowId = windowId;
            Position = new Position();
            ButtonSprite = new SliceSprite("UI/common_button.png", ButtonWidth, 16, true);
            ButtonSprite.Position.Parent = Position;
            TitleText = new Text(1);
            TitleText.Position.Parent = ButtonSprite.Position;
            TitleText.Position.Set(4, 4);
            TitleText.SetText(title);
        }

        public void Draw()
        {
            ButtonSprite.Draw();
            TitleText.Draw();
        }
        
        public void SetActiveColor(bool isActive)
        {
            TitleText.SetColor((byte)(isActive ? 2 : 1));
        }
    }
