using Sirius.Engine;

public class SrSprite
{
    public Position Position;
    public Image Image;
    public int CurrentFrameIndex;
    public bool IsFlip;
    public bool Additive;
    public byte Add;
    public float ScaleY = 1;
    public float ScaleX = 1;
    public int ClipX = 0;
    public int ClipY = 0;
    public int ClipWidth = ScreenHandler.Width;
    public int ClipHeight = ScreenHandler.Height;
    public bool UseCache = true;
    public Position ClipOrigin;

    private int _frameCount;

    public Frame CurrentFrame
    {
        get
        {
            if (UseCache)
            {
                if (CurrentFrameIndex < 0 || CurrentFrameIndex >= Image.FramesCache.Length)
                {
                    Logger.Error($"{CurrentFrameIndex} / {Image.FramesCache.Length}");
                    return null;
                }

                return Image.FramesCache[CurrentFrameIndex];
            }
            else
            {
                if (CurrentFrameIndex < 0 || CurrentFrameIndex >= Image.Frames.Count)
                {
                    Logger.Error($"{CurrentFrameIndex} / {Image.Frames.Count}");
                    return null;
                }

                return Image.Frames[CurrentFrameIndex];
            }
        }
    }

    public SrSprite()
    {
        Position = new Position();
        Image = new Image();
        Image.Frames.Add(new Frame());
    }

    public SrSprite(Position parent)
    {
        Position = new Position(parent);
        Image = new Image();
        Image.Frames.Add(new Frame());
    }

    public void SetClip(int x, int y, int width, int height, Position clipOrigin)
    {
        ClipX = x;
        ClipY = y;
        ClipWidth = width;
        ClipHeight = height;
        ClipOrigin = clipOrigin;
    }

    public void Update()
    {
    }

    public void AnimationUpdate()
    {
        if (_frameCount % Image.AnimationInterval.Value == 0)
        {
            CurrentFrameIndex++;
            if (UseCache)
            {
                if (CurrentFrameIndex >= Image.FramesCache.Length)
                {
                    CurrentFrameIndex = 0;
                }
            }
            else
            {
                if (CurrentFrameIndex >= Image.Frames.Count)
                {
                    CurrentFrameIndex = 0;
                }
            }
        }

        _frameCount++;
        if (_frameCount >= int.MaxValue)
        {
            _frameCount = 0;
        }
    }

    public void Draw()
    {
        Position.GetWorldPosition(out var worldPosX, out var worldPosY);
        var y = worldPosY + Position.Height;
        var frame = CurrentFrame;
        if (UseCache)
        {
            for (var i = 0; i < frame.PixelsCache.Length; i++)
            {
                var pixelPosX = (int)(frame.PixelsCache[i].Point.X * ScaleX) * (IsFlip ? -1 : 1);
                var pixelPosY = (int)(frame.PixelsCache[i].Point.Y * ScaleY);
                var posX = pixelPosX + worldPosX;
                var posY = pixelPosY + y;
                if (ClipOrigin != null)
                {
                    Position.GetRelativePosition(out var localPosX, out var localPosY, ClipOrigin);
                    var localPixelPosX = pixelPosX + localPosX;
                    var localPixelPosY = pixelPosY + localPosY;
                    if (localPixelPosX < ClipX)
                    {
                        continue;
                    }

                    if (localPixelPosX > ClipWidth)
                    {
                        continue;
                    }

                    if (localPixelPosY < ClipY)
                    {
                        continue;
                    }

                    if (localPixelPosY > ClipHeight)
                    {
                        continue;
                    }
                }

                if (Additive)
                {
                    VirtualScreen.Instance.AddPixel(posX, posY, (byte)frame.PixelsCache[i].Color);
                }
                else
                {
                    var col = (byte)(frame.PixelsCache[i].Color + Add);
                    if (col >= 3) col = 3;
                    VirtualScreen.Instance.SetPixel(posX, posY, col);
                }
            }
        }
        else
        {
            for (var i = 0; i < frame.Pixels.Count; i++)
            {
                var posX = (int)(frame.Pixels[i].Point.X * ScaleX) * (IsFlip ? -1 : 1) + worldPosX;
                if (posX < ClipX)
                {
                    continue;
                }

                if (posX > ClipWidth)
                {
                    continue;
                }

                var posY = (int)(frame.Pixels[i].Point.Y * ScaleY) + y;
                if (posY < ClipY)
                {
                    continue;
                }

                if (posY > ClipHeight)
                {
                    continue;
                }

                if (Additive)
                {
                    VirtualScreen.Instance.AddPixel(posX, posY, frame.Pixels[i].Color);
                }
                else
                {
                    var col = (byte)(frame.Pixels[i].Color + Add);
                    if (col >= 3) col = 3;
                    VirtualScreen.Instance.SetPixel(posX, posY, col);
                }
            }
        }
    }
}