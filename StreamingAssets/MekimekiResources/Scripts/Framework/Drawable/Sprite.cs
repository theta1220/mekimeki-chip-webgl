using System.Collections.Generic;
using Sirius.Engine;
using Sirius.Data.Resource;

public class Sprite
{
    public Position Position;
    public byte[] Buffer;
    public int Width;
    public int Height;
    public Frame Frame;
    public Dictionary<string, bool> HitCache;
    public bool Additive = false;
    public bool Alpha = false;
    public int ClipX = 0;
    public int ClipY = 0;
    public int ClipWidth = ScreenHandler.Width;
    public int ClipHeight = ScreenHandler.Height;
    public Position ClipOrigin;
    public byte Add;

    public Sprite(string path, bool alpha)
    {
        Position = new Position();
        var awaiter = ResourceManager.Instance.LoadImageAsync(path).GetAwaiter();
        while (!awaiter.IsCompleted)
        {
        }

        var info = awaiter.GetResult();
        Buffer = info.Buffer;
        Width = info.Width;
        Height = info.Height;
        Frame = new Frame();
        HitCache = new Dictionary<string, bool>();
        Alpha = alpha;

        for (var i = 0; i < Buffer.Length; i++)
        {
            if (Buffer[i] == 255)
            {
                continue;
            }

            var pixel = new Pixel(i % Width, i / Width, Buffer[i]);
            Frame.Pixels.Add(pixel);
            HitCache.Add($"{i % Width},{i / Width}", true);
        }
    }

    public void SetClip(int x, int y, int width, int height, Position clipOrigin)
    {
        ClipX = x;
        ClipY = y;
        ClipWidth = width;
        ClipHeight = height;
        ClipOrigin = clipOrigin;
    }

    public void Draw()
    {
        var clipPosX = 0;
        var clipPosY = 0;
        if (ClipOrigin != null)
        {
            ClipOrigin.GetRelativePosition(out clipPosX, out clipPosY, Position);
        }
        Position.GetWorldPosition(out var posX, out var posY);
        VirtualScreen.Instance.SetPixels(
            posX, posY, Width, Height, Buffer,
            Alpha, Add, clipPosX + ClipX, clipPosY + ClipY,
            ClipWidth, ClipHeight, false);
    }

    public bool IsHitCursor()
    {
        return IsHit(new Point(Input.Instance.GetMousePositionX(), Input.Instance.GetMousePositionY()));
    }

    public bool IsHit(Point pos)
    {
        Position.GetWorldPosition(out var posX, out var posY);

        if (posX <= pos.X &&
            posY <= pos.Y &&
            posX + Width >= pos.X &&
            posY + Height >= pos.Y)
        {
            return true;
        }

        return false;
    }
}