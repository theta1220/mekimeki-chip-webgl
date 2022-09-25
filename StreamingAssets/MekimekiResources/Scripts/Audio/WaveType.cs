
using System;

[Serializable]
public enum WaveType
{
    None = -1,
    Square50 = 0,
    Square25 = 1,
    Square125 = 2,
    Square75 = 3,
    Triangle = 4,
    Triangle2 = 5,
    Saw = 6,
    WhiteNoise = 7,
    PinkNoise = 8,
    Sin = 9,
    Max,
}