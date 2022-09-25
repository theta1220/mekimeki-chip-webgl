using System;

public class HighPassFilter
{
    // フィルタ計算用のバッファ変数。
    double in1 = 0.0f;
    double in2 = 0.0f;
    double out1 = 0.0f;
    double out2 = 0.0f;

    private float cosOmega;
    private float sinOmega;
    private float q;

    public HighPassFilter(float freq, float q, int sampleRate)
    {
        var omega = 2.0f * 3.14159265f * freq / sampleRate;
        sinOmega = (float)Math.Sin(omega);
        cosOmega = (float)Math.Cos(omega);
        this.q = q;
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
        var alpha = sinOmega / (2.0 * q);

        var a0 = 1.0 + alpha;
        var a1 = -2.0 * cosOmega;
        var a2 = 1.0 - alpha;
        var b0 = (1.0 + cosOmega) / 2.0;
        var b1 = -(1.0 + cosOmega);
        var b2 = (1.0 + cosOmega) / 2.0;

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