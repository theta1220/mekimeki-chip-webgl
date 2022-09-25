using System.IO;
using System.Linq;
using NAudio.Wave;

public class WaveExporter
{
    public static void Export(AudioMixer mixer, bool isLoopMode)
    {
        var path = Sirius.Engine.Framework.FileUtil.Manager.SaveFilePanel("wav");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ulong lastBeat =
            (ulong)mixer.Tracks.Max(_ =>
            {
                var lastNote = _.Notes.OrderByDescending(
                    note => note.BeatOffset.Value + note.Length.Value).FirstOrDefault();
                if (lastNote == null) return 0;
                if (!isLoopMode)
                {
                    return lastNote.BeatOffset.Value + lastNote.Length.Value
                                                     + lastNote.WaveVolume.Adsr.ReleaseTime.Value;
                }
                else
                {
                    return lastNote.BeatOffset.Value + lastNote.Length.Value;
                }
            });
        var bpm = mixer.Bpm.Value;
        double sec = bpm / 60.0;
        ulong sampleNum = (ulong)(AudioPlayer.SampleRate / sec) * 4 / 16 * 2;
        ulong allSampleNum = sampleNum * lastBeat;

        mixer.PlayReady();
        var data = new float [allSampleNum];

        for (ulong i = 0; i < (ulong)data.Length; i++)
        {
            var buf = mixer.Calc(i);
            data[i] = buf;
        }

        // 波形作成
        using (var fs = new FileStream(path, FileMode.Create))
        using (var wr = new WaveFileWriter(fs,
            WaveFormat.CreateIeeeFloatWaveFormat(AudioPlayer.SampleRate, 2)))
        {
            wr.WriteSamples(data, 0, data.Length);
        }
    }
}