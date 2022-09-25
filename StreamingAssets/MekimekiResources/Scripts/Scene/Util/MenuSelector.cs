using System.Collections.Generic;
using System.Linq;
using Sirius.Engine;

public class MenuSelector
{
    public Text Text;
    public List<IMenuOption> Options;
    public int SelectIndex;
    public const int MaxLines = 100;
    public bool Controlable = true;
    public Position Position => Text.Position;

    public MenuSelector(List<IMenuOption> options)
    {
        SelectIndex = 0;
        Options = options;
        if (Options == null)
        {
            Options = new List<IMenuOption>();
        }

        Text = new Text(2);
        Text.Position.Set(4, 4);
    }

    public IMenuOption GetOption(string text)
    {
        return Options.FirstOrDefault(_ => _.Name == text);
    }

    public string BuildText()
    {
        var startIndex = 0;
        if (SelectIndex >= MaxLines)
        {
            startIndex = SelectIndex - MaxLines;
        }

        var res = "";
        for (var i = startIndex; i < Options.Count; i++)
        {
            res += $"{(i == SelectIndex && Controlable ? "> " : " ")}{Options[i].BuildText()}";


            if (i - startIndex >= MaxLines)
            {
                break;
            }

            if (i + 1 < Options.Count)
            {
                res += "\n";
            }
        }

        return res;
    }

    public void Update()
    {
        ControlUpdate();
        MouseUpdate();
    }

    public void UpdateBackground()
    {
        Text.SetText(BuildText());
    }

    public void Draw()
    {
        Text.Draw();
    }

    public void ControlUpdate()
    {
        if (!Controlable)
        {
            return;
        }
        if (Input.Instance.Down.IsPushStart)
        {
            SelectIndex++;
            if (SelectIndex >= Options.Count)
            {
                SelectIndex = 0;
            }
        }

        if (Input.Instance.Up.IsPushStart)
        {
            SelectIndex--;
            if (SelectIndex < 0)
            {
                SelectIndex = Options.Count - 1;
            }
        }

        if (Input.Instance.A.IsPushEnd)
        {
            Options[SelectIndex].Invoke(0);
        }

        if (Input.Instance.Right.IsPushStart)
        {
            Options[SelectIndex].Invoke(1);
        }

        if (Input.Instance.Left.IsPushStart)
        {
            Options[SelectIndex].Invoke(-1);
        }
    }

    public void MouseUpdate()
    {
        if (Controlable)
        {
            return;
        }

        var targetChara = GetCharaSprite(Input.Instance.GetMousePositionX(), Input.Instance.GetMousePositionY());
        foreach (var chara in Text.Charas)
        {
            if (chara == targetChara)
            {
                chara.Sprite.Position.Point.Y = 1;
                continue;
            }

            chara.Sprite.Position.Point.Y = 0;
        }

        if (targetChara == null)
        {
            return;
        }

        if (Input.Instance.MouseLeft.IsPushStart)
        {
            var height = Text.CharaHeight + Text.CharaLineInterval;
            var worldPosX = 0;
            var worldPosY = 0;
            Position.GetWorldPosition(out worldPosX, out worldPosY);
            var textLine = (Input.Instance.GetMousePositionY() - worldPosY) / height;
            var lines = Text.SourceText.Split('\n');
            textLine = lines.Length - textLine - 1;
            
            if (targetChara.C == '>')
            {
                Options[textLine].Invoke(1);
            }
            if (targetChara.C == '<')
            {
                Options[textLine].Invoke(-1);
            }
        }
    }

    public Chara GetCharaSprite(int x, int y)
    {
        var width = Text.CharaWidth + Text.CharaInterval;
        var height = Text.CharaHeight + Text.CharaLineInterval;
        var worldPosX = 0;
        var worldPosY = 0;
        Position.GetWorldPosition(out worldPosX, out worldPosY);
        var textIndex = (x - worldPosX) / width;
        var textLine = (y - worldPosY) / height;
        var lines = Text.SourceText.Split('\n');
        textLine = lines.Length - textLine - 1;
        if (lines.Length <= textLine || textLine < 0)
        {
            return null;
        }

        var targetLine = lines[textLine];
        if (targetLine.Length <= textIndex || textIndex < 0)
        {
            return null;
        }

        var index = 0;
        var targetCharaIndex = 0;
        var lineCount = 0;
        foreach (var c in Text.SourceText)
        {
            if (c == '\n')
            {
                index = 0;
                lineCount++;
                continue;
            }

            if (index == textIndex && lineCount == textLine)
            {
                break;
            }

            targetCharaIndex++;
            index++;
        }

        if (targetCharaIndex >= Text.Charas.Count)
        {
            return null;
        }

        return Text.Charas[targetCharaIndex];
    }
}