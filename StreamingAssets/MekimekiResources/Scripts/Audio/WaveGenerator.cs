using System;
using Random = System.Random;

public class WaveGenerator
{
    private Xorshift Xorshift = new Xorshift();
    private System.Random _random = new System.Random();
    private float _whiteNoiseBuff;
    private float _pinkNoiseBuff;

    private RandomTable _whiteNoiseRandomTable;
    private RandomTable _pinkNoiseRandomTable;

    public WaveGenerator()
    {
        _whiteNoiseRandomTable = new RandomTable(32767);
        _pinkNoiseRandomTable = new RandomTable(127);
    }

    public float Generate(int noteMelody, double r, double count, WaveType waveType)
    {
        switch (waveType)
        {
            case WaveType.Square50:
                return CalcSquare(0.5, count, r);
            
            case WaveType.Square25:
                return CalcSquare(0.25, count, r);
            
            case WaveType.Square125:
                return CalcSquare(0.125, count, r);
            
            case WaveType.Square75:
                return CalcSquare(0.75, count, r);
            
            case WaveType.Triangle:
            {
                var time = (count % r / r);
                return (float)(((time > 0.5 ? time : 1.0f - time) - 0.75f) * 4);
            }
            
            case WaveType.Triangle2:
            {
                var time = (count % r / r);
                time = Math.Round(time * 8) / 8.0f;
                return (float)(((time > 0.5 ? time : 1.0f - time) - 0.75f) * 4);
            }
            
            case WaveType.Saw:
            {
                var time = (count % r / r);
                time = Math.Round(time * 8) / 8.0f;
                return (float)((1.0 - time - 0.5) * 2);
            }

            case WaveType.WhiteNoise:
            {
                var sample = (AudioPlayer.SampleRate / (double)24000) * 128 / 200;
                if ((long)sample == 0 || count % (ulong)sample == 0)
                {
                    //_whiteNoiseBuff = _whiteNoiseRandomTable.Get((int)count);
                    //_whiteNoiseBuff = (float)_random.NextDouble() * 2 - 1;
                    _whiteNoiseBuff = (float)Xorshift.NextDouble() * 2 - 1;
                }

                return _whiteNoiseBuff;
            }

            case WaveType.PinkNoise:
            {
                if (noteMelody == 0) noteMelody = 1;
                
                var sample =
                    (AudioPlayer.SampleRate / (double)24000)
                    * 128 / (noteMelody * noteMelody);

                if ((long)sample == 0 || count % (ulong)sample == 0)
                {
                    //_pinkNoiseBuff = _pinkNoiseRandomTable.Get((int)count);
                    //_pinkNoiseBuff = (float)_random.NextDouble() * 2 - 1;
                    _pinkNoiseBuff = (float)Xorshift.NextDouble() * 2 - 1;
                }

                return _pinkNoiseBuff;
            }

            case WaveType.Sin:
            {
                var time = (count % r / r);
                var buf = (float)Math.Sin(time * 2 * Math.PI) * 2;
                return buf;
            }
        }

        return 0;
    }

    private float CalcSquare(double duty, double count, double r)
    {
        var time = count % r / r;
        return time > duty ? 1.0f : -1.0f;
    }
}