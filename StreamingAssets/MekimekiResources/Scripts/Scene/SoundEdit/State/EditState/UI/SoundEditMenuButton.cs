using System;
using Sirius.Engine;

public class SoundEditMenuButton
{
    public string Title;
    public Action Action;

    public Position Root;
    public Text TitleText;
    public SrSprite ButtonSprite;

    public const int Width = 73;
    public const int Height = 21;

    public Point OriginPoint;

    public SoundEditMenuButton(Position parent, string title, Action action)
    {
        Title = title;
        Action = action;

        Root = new Position();
        ButtonSprite = new SrSprite();
        ButtonSprite.Image = ResourceManager.Instance.LoadFromJson<Image>("SoundEdit/menu_button.json");
        ButtonSprite.Position.Parent = Root;
        TitleText = new Text(0);
        TitleText.SetText(title);
        TitleText.Position.Parent = ButtonSprite.Position;
        TitleText.Position.Set(5, 7);
        Root.Parent = parent;

        OriginPoint = new Point();
    }

    public void SetPosition(int x, int y)
    {
        OriginPoint.X = x;
        OriginPoint.Y = y;
        
        Root.Set(x, y);
    }

    public void Update()
    {
        var mousePos = new Point();
        mousePos.X = Input.Instance.GetMousePositionX();
        mousePos.Y = Input.Instance.GetMousePositionY();
        int gridPosX;
        int gridPosY;
        Root.GetWorldPosition(out gridPosX, out gridPosY);
        var clickPosX = Input.Instance.GetMousePositionX() - gridPosX;
        var clickPosY = Input.Instance.GetMousePositionY() - gridPosY;

        // hit
        if (clickPosX > 0 && clickPosX < Width &&
            clickPosY > 0 && clickPosY < Height)
        {
            Root.Set(OriginPoint.X, OriginPoint.Y + 1);
            if (Input.Instance.MouseLeft.IsPush)
            {
                ButtonSprite.Add = 1;
            }
            else
            {
                ButtonSprite.Add = 0;
            }

            if (Input.Instance.MouseLeft.IsPushEnd)
            {
                Action.Invoke();
            }
        }
        else
        {
            Root.Set(OriginPoint.X, OriginPoint.Y);
        }
    }

    public void Draw()
    {
        ButtonSprite.Draw();
        TitleText.Draw();
    }
}