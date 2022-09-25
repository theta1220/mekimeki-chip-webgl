

using Microsoft.CodeAnalysis.Operations;
using Sirius.Engine;

public class SystemWindow
{
    public const int WindowBarHeight = 12;
    public Position Position;
    public Position InnerPosition;
    public SliceSprite WindowBar;
    public SliceSprite WindowFrame;
    public string Title;
    public Text TitleText;
    public Point WindowMoveCursorOffset;
    public bool IsWindowMove;
    public int DrawPriority;
    public int WindowWidth;
    public int WindowHeight;
    public bool Visible;
    public Sprite CloseButton;

    public SystemWindow(string title, int width, int height, bool drawCenter)
    {
        DrawPriority = 0;
        Title = title;
        TitleText = new Text(2);
        TitleText.SetText(title);
        Position = new Position();
        WindowWidth = width;
        WindowHeight = height;
        
        WindowBar = new SliceSprite("UI/window_bar.png", WindowWidth, WindowBarHeight, true);
        WindowFrame = new SliceSprite("UI/window_frame.png", WindowWidth, WindowHeight, drawCenter);

        WindowBar.Position.Parent = Position;
        TitleText.Position.Parent = WindowBar.Position;
        TitleText.Position.Set(4, 1);
        WindowFrame.Position.Parent = Position;

        WindowBar.Position.Point.Y = WindowHeight - 1;
        IsWindowMove = false;

        InnerPosition = new Position();
        InnerPosition.Parent = Position;
        InnerPosition.Set(2,2);
        
        CloseButton = new Sprite("UI/close_button.png", true);
        CloseButton.Position.Parent = WindowBar.Position;
        CloseButton.Position.Set(WindowWidth - 12, 1);

        Visible = true;
    }

    public void Update()
    {
    }

    public void UpdateBackground()
    {
        if (Input.Instance.MouseLeft.IsPushStartPure && CloseButton.IsHitCursor())
        {
            Visible = false;
        }
    }

    public bool MoveWindow()
    {
        var moved = false;
        var cursorPos = new Point(Input.Instance.GetMousePositionX(), Input.Instance.GetMousePositionY());
        if (Input.Instance.MouseLeft.IsPushStartPure && WindowBar.IsHit(cursorPos))
        {
            IsWindowMove = true;
            WindowMoveCursorOffset = new Point();
            WindowMoveCursorOffset.X = cursorPos.X - Position.Point.X;
            WindowMoveCursorOffset.Y = cursorPos.Y - Position.Point.Y;
            moved = true;
        }

        if (IsWindowMove && !Input.Instance.MouseLeft.IsPush)
        {
            IsWindowMove = false;
        }
        
        if (IsWindowMove)
        {
            Position.Point.X = cursorPos.X - WindowMoveCursorOffset.X;
            Position.Point.Y = cursorPos.Y - WindowMoveCursorOffset.Y;

            if (Position.Point.X < 0)
            {
                Position.Point.X = 0;
            }

            if (Position.Point.Y < 0)
            {
                Position.Point.Y = 0;
            }

            if (Position.Point.X + WindowWidth > Screen.Width)
            {
                Position.Point.X = Screen.Width - WindowWidth;
            }

            if (Position.Point.Y + WindowHeight + WindowBarHeight > Screen.Height)
            {
                Position.Point.Y = Screen.Height - WindowHeight - WindowBarHeight;
            }

            moved = true;
        }

        return moved;
    }

    public bool Draw()
    {
        if (!Visible)
        {
            return false;
        }
        WindowFrame.Draw();
        WindowBar.Draw();
        TitleText.Draw();
        CloseButton.Draw();

        return true;
    }

    public bool IsHitCursor()
    {
        var pos = new Point(Input.Instance.GetMousePositionX(), Input.Instance.GetMousePositionY());
        var width = WindowWidth;
        var height = WindowHeight + WindowBarHeight;

        if (pos.X > Position.Point.X &&
            pos.Y > Position.Point.Y &&
            pos.X < Position.Point.X + width &&
            pos.Y < Position.Point.Y + height)
        {
            return true;
        }

        return false;
    }
}