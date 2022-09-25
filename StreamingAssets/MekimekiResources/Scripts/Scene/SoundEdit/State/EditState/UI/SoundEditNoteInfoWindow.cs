public class SoundEditNoteInfoWindow
{
    public Position Root;
    public SrSprite WindowSprite;
    public Text Text;
    public bool Visible;
    public SoundEditCursor Cursor;
    public AudioMixer AudioMixer;
    public ReferenceValue<int> EditingTrackIndex;

    public SoundEditNoteInfoWindow(AudioMixer audioMixer, ReferenceValue<int> editingTrackIndex, SoundEditCursor cursor)
    {
        AudioMixer = audioMixer;
        Cursor = cursor;
        EditingTrackIndex = editingTrackIndex;
        Root = new Position(Cursor.CursorSprite.Position);

        WindowSprite = new SrSprite(Root);
        WindowSprite.Image = ResourceManager.Instance
            .LoadFromJson<Image>("SoundEdit/note_info_window.json");

        Text = new Text(2, CharaType.En);
        Text.Position.Parent = WindowSprite.Position;
        Text.Position.Set(-13, 19);
    }

    public void Update()
    {
        var beat = Cursor.GridX + Cursor.CurrentPage * SoundEditGrid.GridWidth;
        var track = AudioMixer.Tracks[EditingTrackIndex.Value];
        SoundNote targetNote = null;
        foreach (var note in track.Notes)
        {
            if (note.BeatOffset.Value == beat && note.Melody.Value == Cursor.GridY)
            {
                targetNote = note;
                break;
            }
        }

        if (targetNote == null)
        {
            Visible = false;
            return;
        }

        Visible = true;
        Text.SetText(
            $"A{targetNote.WaveVolume.Adsr.AttackTime.Value*100:000}" +
            $"D{targetNote.WaveVolume.Adsr.DecayTime.Value*100:000}\n" +
            $"S{targetNote.WaveVolume.Adsr.SustainLevel.Value*100:000}" +
            $"R{targetNote.WaveVolume.Adsr.ReleaseTime.Value*100:000}\n" +
            $"V{targetNote.WaveVolume.Volume.Value*100:000}" +
            $"P{targetNote.WaveVolume.Pan.Value*100:000}");
        Text.SetCharaColor("ADSRVP", 3);
    }

    public void Draw()
    {
        if (Visible)
        {
            WindowSprite.Draw();
            Text.Draw();
        }
    }
}