using Sirius.Engine;

public class Rect
{
    public Position Position;
    public int Width;
    public int Height;
    public bool AddBlend;
    public byte[] Buffer;
    public int ClipX = 0;
    public int ClipY = 0;
    public int ClipWidth = ScreenHandler.Width;
    public int ClipHeight = ScreenHandler.Height;
    public Position ClipOrigin;
    public byte Color;

    public Rect(int width, int height, byte color)
    {
        Position = new Position();
        Width = width;
        Height = height;
        Color = color;
        
        SetWidth(width);
    }

    public void Draw()
    {
        if (Width <= 0 || Height <= 0)
        {
            return;
        }

        var clipX = 0;
        var clipY = 0;
        if (ClipOrigin != null)
        {
            ClipOrigin.GetRelativePosition(out clipX, out clipY, Position);
        }

        Position.GetWorldPosition(out var worldPosX, out var worldPosY);

        VirtualScreen.Instance.SetPixels(
            worldPosX, worldPosY, Width, Height, Buffer,
            true, 0, clipX + ClipX, clipY + ClipY, ClipWidth, ClipHeight, AddBlend);
    }

    public void SetWidth(int width)
    {
        if (width < 0)
        {
            width = 0;
        }
        Width = width;
        Buffer = new byte[Width * Height];
        for (var i = 0; i < Buffer.Length; i++)
        {
            Buffer[i] = Color;
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
}