using System;
using System.Text;
using Sirius.Engine;

public class SoundEditCursor
{
    public Sprite CursorSprite;
    public Position Root;
    public Position Origin;

    public int GridX;
    public int GridY;

    public int CurrentPageCursor = 0;
    public int CurrentPage => CurrentPageCursor;

    public SoundEditCursor(Position root, int windowWidth, int windowHeight)
    {
        Root = root;
        Origin = new Position(root);
        Origin.Set(-3, -3);
        CursorSprite = new Sprite("SoundEdit/cursor.png", true);
        CursorSprite.Position.Parent = Origin;
        CursorSprite.SetClip(Root.Point.X, Root.Point.Y, windowWidth, windowHeight, root);
    }

    public void Update()
    {
        var mousePosX = Input.Instance.GetMousePositionX();
        var mousePosY = Input.Instance.GetMousePositionY();
        Root.GetWorldPosition(out var gridPosX, out var gridPosY);
        var clickPosX = mousePosX - gridPosX;
        var clickPosY = mousePosY - gridPosY;
        GridX = clickPosX / SoundEditNote.GridSize;
        GridY = clickPosY / SoundEditNote.GridSize;
        CursorSprite.Position.Set(
            GridX * SoundEditNote.GridSize,
            GridY * SoundEditNote.GridSize);
        
        if (Input.Instance.Left.IsPushStart)
        {
            CurrentPageCursor--;
        }

        if (Input.Instance.Right.IsPushStart)
        {
            CurrentPageCursor++;
        }

        var mouseWheel = Input.Instance.GetMouseWheel();
        if (mouseWheel!= 0)
        {
            CurrentPageCursor -= mouseWheel / Math.Abs(mouseWheel);
        }

        if (CurrentPageCursor < 0)
        {
            CurrentPageCursor = 0;
        }
    }

    public void Draw()
    {
        CursorSprite.Draw();
    }
}