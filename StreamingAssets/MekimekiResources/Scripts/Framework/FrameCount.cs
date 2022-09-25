    public static class FrameCount
    {
        public static int Value;

        public static void Update()
        {
            Value++;
            if (Value >= int.MaxValue)
            {
                Value = 0;
            }
        }
    }
