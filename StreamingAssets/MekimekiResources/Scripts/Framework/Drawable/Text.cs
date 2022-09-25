using System.Collections.Generic;
using System.Linq;
using Sirius.Engine;

public class Text
{
    public Position Position;
    public string SourceText;
    public List<Chara> Charas;
    public byte Color;
    public CharaType CharaType;

    public int CharaWidth = 6;
    public int CharaHeight = 7;
    public int CharaInterval = 1;
    public int CharaLineInterval = 6;

    public int CharaWidthEn = 3;
    public int CharaHeightEn = 6;
    public int CharaIntervalEn = 1;
    public int CharaLineIntervalEn = 3;

    public static Stack<Chara> PoolJa { get; set; } = new Stack<Chara>();
    public static Stack<Chara> PoolEn { get; set; } = new Stack<Chara>();

    public int SizeXJa
    {
        get
        {
            var size = CharaWidth + CharaInterval;
            return SourceText.Split('\n').Max(_ => _.Length) * size;
        }
    }

    public int SizeYEn
    {
        get
        {
            var size = CharaHeight + CharaLineInterval;
            return (SourceText.Count(_ => _ == '\n') + 1) * size;
        }
    }

    public Text(byte color, CharaType charaType = CharaType.Ja)
    {
        Position = new Position();
        Charas = new List<Chara>();
        Color = color;

        CharaType = charaType;
        if (CharaType == CharaType.Ja)
        {
            CharaWidth = 6;
            CharaHeight = 7;
            CharaInterval = 1;
            CharaLineInterval = 6;
        }

        if (charaType == CharaType.En)
        {
            CharaWidth = 3;
            CharaHeight = 5;
            CharaInterval = 1;
            CharaLineInterval = 1;
        }
    }

    public void SetCharaColor(string chara, byte color)
    {
        foreach (var c in chara)
        {
            foreach (var ch in Charas)
            {
                if (ch.C == c)
                {
                    ch.SetColor(color);
                    break;
                }
            }
        }
    }

    public void SetText(string text)
    {
        if (SourceText == text)
        {
            return;
        }

        SourceText = text;

        AddPool(Charas);
        Charas.Clear();
        int y = -CharaHeight;
        int count = 0;
        foreach (var c in text)
        {
            if (c == '\r') continue;
            if (c == '\n')
            {
                y -= CharaHeight + CharaLineInterval;
                count = 0;
                continue;
            }

            {
                var chara = CreateChara(c, Color, CharaType);
                chara.Position.Parent = Position;
                chara.Position.Point.X = (CharaWidth + CharaInterval) * count;
                chara.Position.Point.Y = y;
                Charas.Add(chara);
            }

            if (Chara.Dakuten.Contains(c))
            {
                var chara = CreateChara('゛', Color);
                chara.Position.Parent = Position;
                chara.Position.Point.X = (CharaWidth + CharaInterval) * count + 1;
                chara.Position.Point.Y = y + CharaHeight;
                Charas.Add(chara);
            }

            if (Chara.HanDakuten.Contains(c))
            {
                var chara = CreateChara('゜', Color);
                chara.Position.Parent = Position;
                chara.Position.Point.X = (CharaWidth + CharaInterval) * count + 1;
                chara.Position.Point.Y = y + CharaHeight;
                Charas.Add(chara);
            }

            count++;
        }

        foreach (var chara in Charas)
        {
            chara.Position.Point.Y -= y;
        }

        SetColor(Color);
    }

    public void SetColor(byte color)
    {
        Color = color;
        foreach (var chara in Charas)
        {
            chara.SetColor(color);
        }
    }

    public void Draw()
    {
        foreach (var chara in Charas)
        {
            chara.SetColor(Color);
            chara.Draw();
        }
    }

    private void AddPool(List<Chara> charas)
    {
        foreach (var chara in charas)
        {
            if (chara.CharaType == CharaType.Ja)
            {
                PoolJa.Push(chara);
            }

            if (chara.CharaType == CharaType.En)
            {
                PoolEn.Push(chara);
            }
        }
    }

    private Chara CreateChara(char c, byte color, CharaType charaType = CharaType.Ja)
    {
        if (charaType == CharaType.Ja)
        {
            if (PoolJa.Count > 0)
            {
                var obj = PoolJa.Pop();
                obj.Initialize(c, color);
            }
            return new Chara(c, color, CharaType.Ja);
        }
        if (charaType == CharaType.En)
        {
            if (PoolEn.Count > 0)
            {
                var obj = PoolEn.Pop();
                obj.Initialize(c, color);
            }
            return new Chara(c, color, CharaType.En);
        }

        return new Chara(c, color, charaType);
    }
}