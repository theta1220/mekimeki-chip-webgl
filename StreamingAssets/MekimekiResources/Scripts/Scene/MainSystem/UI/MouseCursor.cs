    using Sirius.Engine;

    public class MouseCursor
    {
        public Position Position;
        public Sprite Sprite;
        
        public MouseCursor()
        {
            Position = new Position();
            Sprite = new Sprite("UI/cursor.png", true);
            Sprite.Position.Parent = Position;
            Sprite.Position.Set(0, -Sprite.Height);
        }

        public void Update()
        {
            var x = Input.Instance.GetMousePositionX();
            var y = Input.Instance.GetMousePositionY();
            Position.Set(x, y);
        }

        public void Draw()
        {
            Sprite.Draw();
        }
    }
