    using System.Diagnostics;

    public class ApplicationInfoWindow : WindowSystemBase
    {
        public const int FPSViewCountMax = 10;
        public Text FPSText;
        public Stopwatch Stopwatch;
        public float FPS;
        public int FPSViewCount;
        
        public ApplicationInfoWindow() : base("info", 80, 20, true)
        {
            FPSText = new Text(2);
            FPSText.Position.Parent = Window.InnerPosition;
            FPSText.Position.Set(4, 4);
            Window.Position.Set(5, 100);
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            FPSViewCount = FPSViewCountMax;
        }

        public override void UpdateBackground()
        {
            base.UpdateBackground();
            Stopwatch.Stop();
            FPS += 1000f / Stopwatch.ElapsedMilliseconds;
            Stopwatch.Restart();

            FPSViewCount--;
            if (FPSViewCount == 0)
            {
                FPS /= FPSViewCountMax;
                FPSViewCount = FPSViewCountMax;
                FPSText.SetText($"fps {FPS : 00.0}");
            }
        }

        public override bool Draw()
        {
            if (!base.Draw())
            {
                return false;
            }
            FPSText.Draw();
            return true;
        }
    }
