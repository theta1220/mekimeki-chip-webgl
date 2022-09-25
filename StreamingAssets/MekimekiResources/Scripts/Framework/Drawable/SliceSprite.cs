using System.Collections.Generic;
using Sirius.Engine;

public class SliceSprite
{
    public Position Position;
    public byte[] Buffer;
    public int Width;
    public int Height;
    public bool Additive = false;
    public int RectWidth;
    public int RectHeight;
    public bool DrawCenter;
    public byte[] BuildCenterBuffer;

    public SliceSprite(string path, int rectWidth, int rectHeight, bool drawCenter)
    {
        SetRectSize(rectWidth, rectHeight);
        Position = new Position();
        var info = ResourceManager.Instance.LoadImage(path);
        Buffer = info.Buffer;
        Width = info.Width;
        Height = info.Height;
        DrawCenter = drawCenter;
        var sideWidth = Width / 3;
        var sideHeight = Height / 3;
        var centerWidth = RectWidth - sideWidth * 2;
        var centerHeight = RectHeight - sideHeight * 2;
        BuildCenterBuffer = new byte[centerWidth * centerHeight];

        for (var y = 0; y < centerHeight; y++)
        {
            for (var x = 0; x < centerWidth; x++)
            {
                var bufPosX = x % sideWidth + 1 * sideWidth;
                var bufPosY = y % sideHeight + 1 * sideHeight;
                var index = bufPosX + bufPosY * Width;
                var buffer = Buffer[index];

                BuildCenterBuffer[x + y * centerWidth] = buffer;
            }
        }
    }

    public void SetRectSize(int width, int height)
    {
        RectWidth = width;
        RectHeight = height;
    }

    public void Draw()
    {
        var posX = 0;
        var posY = 0;
        Position.GetWorldPosition(out posX, out posY);

        var sideWidth = Width / 3;
        var sideHeight = Height / 3;
        var centerWidth = RectWidth - sideWidth * 2;
        var centerHeight = RectHeight - sideHeight * 2;

        if (DrawCenter)
        {
            VirtualScreen.Instance.SetPixels(
                posX + sideWidth, posY + sideHeight, centerWidth, centerHeight, BuildCenterBuffer,
                false, 0, 0, 0, Screen.Width, Screen.Height, false);
        }
        for (var y = 0; y < RectHeight; y++)
        {
            for (var x = 0; x < RectWidth; x++)
            {
                var _x = x;
                var _y = y;
                var rectPosX = 0;
                var rectPosY = 0;
                
                if (x >= Width / 3)
                {
                    rectPosX = 1;
                }

                if (x >= RectWidth - Width / 3)
                {
                    _x -= RectWidth - Width / 3;
                    rectPosX = 2;
                }
                if (y >= Height / 3)
                {
                    rectPosY = 1;
                }

                if (y >= RectHeight - Height / 3)
                {
                    _y -= RectHeight - Height / 3;
                    rectPosY = 2;
                }

                if (rectPosX == 1 && rectPosY == 1)
                {
                    continue;
                }

                var bufPosX = _x % (Width / 3) + rectPosX * (Width / 3);
                var bufPosY = _y % (Height / 3) + rectPosY * (Height / 3);
                var index = bufPosX + bufPosY * Width;
                var buffer = Buffer[index];

                if (buffer == 255)
                {
                    continue;
                }

                VirtualScreen.Instance.SetPixel(posX + x, posY + y, buffer);
            }
        }
    }

    public bool IsHitCursor()
    {
        return IsHit(new Point(Input.Instance.GetMousePositionX(), Input.Instance.GetMousePositionY()));
    }

    public bool IsHit(Point pos)
    {
        var posX = 0;
        var posY = 0;
        Position.GetWorldPosition(out posX, out posY);

        if (posX <= pos.X &&
            posY <= pos.Y &&
            posX + RectWidth >= pos.X &&
            posY + RectHeight >= pos.Y)
        {
            return true;
        }

        return false;
    }
}