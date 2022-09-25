using System;

public class WaveMonitorWindow : WindowSystemBase
{
    private float[] _cache;
    
    public WaveMonitorWindow() : base("wave", 64 + 3, 64, true)
    {
        Window.Position.Set(5, 270);
    }

    public override bool Draw()
    {
        if (!base.Draw())
        {
            return false;
        }

        Window.InnerPosition.GetWorldPosition(out var rootPosX, out var rootPosY);
        var buffer = AudioPlayer.Instance.GetBuffer();
        var scale = 12;
        var waveSize = 48;
        if (buffer.Length > 64 * scale)
        {
            _cache = buffer;
            AudioPlayer.Instance.RemoveBuffer();
        }

        if (_cache == null)
        {
            return true;
        }
        for (var i = 0; i < 64; i++)
        {
            var buf = (int)(_cache[i * scale] * waveSize);
            if (buf < -waveSize || buf >= waveSize)
            {
                continue;
            }

            var size = Math.Abs(buf);
            var dir = size == 0 ? 0 : buf / size;
            for (var y = 0; y < size / 2; y++)
            {
                VirtualScreen.Instance.SetPixel(rootPosX + i, rootPosY + 32 + y * dir, 2);
            }
        }

        return true;
    }
}