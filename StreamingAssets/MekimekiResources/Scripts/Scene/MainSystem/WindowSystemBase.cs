    using System;

    public abstract class WindowSystemBase
    {
        public int DrawPriority;
        public SystemWindow Window;
        public Guid Id;

        public WindowSystemBase(string title, int width, int height, bool drawCenter)
        {
            Id = Guid.NewGuid();
            Window = new SystemWindow(title, width, height, drawCenter);
        }

        public virtual void Update()
        {
            Window.Update();
        }

        public virtual void UpdateBackground()
        {
            Window.UpdateBackground();
        }

        public bool MoveWindow()
        {
            return Window.MoveWindow();
        }

        public virtual bool Draw()
        {
            return Window.Draw();
        }
    }
