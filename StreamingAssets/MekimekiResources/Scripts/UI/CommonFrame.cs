using System.Collections.Generic;

public class CommonFrame
{
    public Position Position;
    public int Width;
    public int Height;
    public int SizeX => Width * 8;
    public int SizeY => Height * 8;

    public List<Sprite> Sprites;
    
    public CommonFrame()
    {
        Position = new Position();
        Sprites = new List<Sprite>();
    }

    public void SetSize(int width, int height)
    {
        if (Width == width && Height == height)
        {
            return;
        }
        Width = width;
        Height = height;
        
        Sprites.Clear();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var i = 0;
                if (x == 0 && y == 0) i = 0;
                else if (x == width - 1 && y == 0) i = 2;
                else if (x == 0 && y == Height - 1) i = 6;
                else if (x == width - 1 && y == Height - 1) i = 8;
                else if (y == 0) i = 1;
                else if (x == 0) i = 3;
                else if (x == Width - 1) i = 5;
                else if (y == Height - 1) i = 7;
                else i = 4;
                
                var sprite = new Sprite($"UI/frame9/{i}.png", true);
                sprite.Position.Parent = Position;
                sprite.Position.Set(x * sprite.Width, -(y + 1) * sprite.Height + sprite.Height * Height);
                Sprites.Add(sprite);
            }
        }
    }

    public void Draw()
    {
        foreach (var sprite in Sprites)
        {
            sprite.Draw();
        }
    }
}