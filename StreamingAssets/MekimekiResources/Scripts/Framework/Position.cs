    public class Position
    {
        public Position Parent;
        public int Height;
        public Point Point;

        public Position()
        {
            Point = new Point();
        }

        public Position(Position parent)
        {
            Point = new Point();
            Parent = parent;
        }

        public Position(int x, int y)
        {
            Point = new Point();
            Point.X = x;
            Point.Y = y;
        }

        public void Set(int x, int y)
        {
            Point.X = x;
            Point.Y = y;
        }

        public void GetWorldPosition(out int x, out int y)
        {
            x = 0;
            y = 0;
            GetWorldPositionInternal(ref x, ref y);
        }

        private void GetWorldPositionInternal(ref int x, ref int y)
        {
            if (Parent != null)
            {
                Parent.GetWorldPositionInternal(ref x, ref y);
            }

            x += Point.X;
            y += Point.Y;
        }

        public void GetRelativePosition(out int x, out int y, Position target)
        {
            x = 0;
            y = 0;
            GetRelativePositionInternal(ref x, ref y, target);
        }

        private void GetRelativePositionInternal(ref int x, ref int y, Position target)
        {
            if (Parent != null && Parent != target)
            {
                Parent.GetRelativePositionInternal(ref x, ref y, target);
            }

            x += Point.X;
            y += Point.Y;
        }
    }
