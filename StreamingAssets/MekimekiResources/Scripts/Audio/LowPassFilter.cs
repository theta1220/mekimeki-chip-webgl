using System;

public class LowPassFilter
{
    // フィルタ計算用のバッファ変数。
    double in1 = 0.0f;
    double in2 = 0.0f;
    double out1 = 0.0f;
    double out2 = 0.0f;

    private float _freq;
    private float _q;
    private float _sinOmega;
    private float _cosOmega;

    public LowPassFilter(float freq, float q, int sampleRate)
    {
        var omega = 2.0f * 3.14159265f * freq / sampleRate;
        _freq = freq;
        _q = q;
        _sinOmega = (float)Math.Sin(omega);
        _cosOmega = (float)Math.Cos(omega);
    }

    public void PlayReady()
    {
        in1 = 0;
        in2 = 0;
        out1 = 0;
        out2 = 0;
    }

    public float Calc(float input)
    {
        // フィルタ係数を計算する
        var alpha = _sinOmega / (2.0f * _q);

        var a0 = 1.0f + alpha;
        var a1 = -2.0f * _cosOmega;
        var a2 = 1.0f - alpha;
        var b0 = (1.0f - _cosOmega) / 2.0f;
        var b1 = 1.0f - _cosOmega;
        var b2 = (1.0f - _cosOmega) / 2.0f;

        // フィルタを適用
        // 入力信号にフィルタを適用し、出力信号として書き出す。
        var output = b0 / a0 * input + b1 / a0 * in1 + b2 / a0 * in2
                     - a1 / a0 * out1 - a2 / a0 * out2;

        in2 = in1; // 2つ前の入力信号を更新
        in1 = input; // 1つ前の入力信号を更新

        out2 = out1; // 2つ前の出力信号を更新
        out1 = output; // 1つ前の出力信号を更新
        return (float)output;
    }
}