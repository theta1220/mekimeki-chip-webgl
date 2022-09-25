    using Sirius.Engine;

    public class ToggleButton
    {
        public Position Root;
        public SrSprite TrueSprite;
        public SrSprite FalseSprite;
        public Point ButtonSize;
        public ReferenceValue<bool> Value;

        public ToggleButton(string truePath, string falsePath, 
            Point buttonSize, ReferenceValue<bool> value)
        {
            Root = new Position();
            TrueSprite = new SrSprite(Root);
            TrueSprite.Image = ResourceManager.Instance
                .LoadFromJson<Image>(truePath);
            FalseSprite = new SrSprite(Root);
            FalseSprite.Image = ResourceManager.Instance
                .LoadFromJson<Image>(falsePath);
            ButtonSize = buttonSize;
            Value = value;
        }

        public void Update()
        {
            var buttonPosX = 0;
            var buttonPosY = 0;
            var mousePosX = Input.Instance.GetMousePositionX();
            var mousePosY = Input.Instance.GetMousePositionY();
            Root.GetWorldPosition(out buttonPosX, out buttonPosY);

            if (Input.Instance.MouseLeft.IsPushStartPure)
            {
                if (mousePosX >= buttonPosX &&
                    mousePosY >= buttonPosY &&
                    mousePosX <= buttonPosX + ButtonSize.X &&
                    mousePosY <= buttonPosY + ButtonSize.Y)
                {
                    Value.Value = !Value.Value;
                }
            }
        }

        public void Draw()
        {
            if (Value.Value)
            {
                TrueSprite.Draw();
            }
            else
            {
                FalseSprite.Draw();
            }
        }
    }
