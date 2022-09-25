using System;
using System.Collections.Generic;

public class SoundEditMenuList
{
    public List<SoundEditMenuButton> Buttons;
    public Position Root;

    public SoundEditMenuList()
    {
        Root = new Position();
        Buttons = new List<SoundEditMenuButton>();
    }

    public void AddButton(string title, Action action)
    {
        var button = new SoundEditMenuButton(Root, title, action);
        button.SetPosition(0, (int)(Buttons.Count * SoundEditMenuButton.Height * 1.1));
        Buttons.Add(button);
    }

    public void Update()
    {
        foreach (var soundEditMenuButton in Buttons)
        {
            soundEditMenuButton.Update();
        }
    }

    public void Draw()
    {
        foreach (var soundEditMenuButton in Buttons)
        {
            soundEditMenuButton.Draw();
        }
    }
}