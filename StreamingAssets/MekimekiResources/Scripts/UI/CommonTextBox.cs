    using Sirius.Engine;

    public class CommonTextBox : IDrawable
    {
        protected int _sorting;
        public int Sorting => _sorting;
        public Position Position;
        public CommonFrame Frame;
        public Text Text;

        public CommonTextBox(int sorting)
        {
            _sorting = sorting;
            Position = new Position();
            Frame = new CommonFrame();
            Frame.SetSize(20, 5);
            Frame.Position.Parent = Position;
            Text = new Text(0);
            Text.Position.Parent = Frame.Position;
            Text.Position.Set(8, 4);
        }

        public void SetFrame(bool autoPositionSet = true)
        {
            if (string.IsNullOrEmpty(Text.SourceText))
            {
                return;
            }

            Frame.SetSize(Text.SizeXJa / 8 + 3, Text.SizeYEn / 8 + 3);
            Text.Position.Point.X = (Frame.SizeX - Text.SizeXJa) / 2;
            Text.Position.Point.Y = (Frame.SizeY - Text.SizeYEn) / 2 + 4;
            if (autoPositionSet && Position.Parent == null)
            {
                Position.Set(10, ScreenHandler.Height - Frame.SizeY - 10);
            }
        }

        public void Draw()
        {
            Frame.Draw();
            Text.Draw();
        }
    }
