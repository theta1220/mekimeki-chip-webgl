using System;
using System.Collections.Generic;
using System.Linq;
using Sirius.Engine;

public class SystemDockBar
{
    public Position Position;
    public SliceSprite BarSprite;

    public List<SystemDockButton> Buttons;

    public SystemDockBar(List<WindowSystemBase> windowSystems)
    {
        Position = new Position();
        BarSprite = new SliceSprite("UI/window_bar.png", Screen.Width, 23, true);
        BarSprite.Position.Parent = Position;

        Buttons = new List<SystemDockButton>();
        int count = 0;
        foreach (var windowSystem in windowSystems)
        {
            var button = new SystemDockButton(windowSystem.Id, windowSystem.Window.Title);
            button.Position.Set(count * (SystemDockButton.ButtonWidth + 2) + 5, 3);
            Buttons.Add(button);
            count++;
        }
    }

    public Guid GetTargetWindowId()
    {
        var cursorX = Input.Instance.GetMousePositionX();
        var cursorY = Input.Instance.GetMousePositionY();

        foreach (var button in Buttons)
        {
            if (button.ButtonSprite.IsHit(new Point(cursorX, cursorY)))
            {
                return button.WindowId;
            }
        }

        return Guid.Empty;
    }

    public void SetActiveColor(Guid targetWindowId)
    {
        foreach (var button in Buttons)
        {
            if (button.WindowId == targetWindowId)
            {
                button.SetActiveColor(true);
            }
            else
            {
                button.SetActiveColor(false);
            }
        }
    }

    public void Draw()
    {
        BarSprite.Draw();

        int count = 0;
        foreach (var button in Buttons)
        {
            button.Draw();
        }
    }
}