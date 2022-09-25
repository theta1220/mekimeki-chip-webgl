using System.Collections.Generic;
using System.Linq;
using NClone;
using Sirius.Engine;

public class SoundEditSelectRect
{
    public Rect SelectAreaRect;
    public int Start = 0;
    public int StartGridX = 0;
    public int End = 0;
    public int Offset = 0;
    public AudioMixer AudioMixer;
    public ReferenceValue<int> EditingTrackIndex;
    public SoundEditCursor Cursor;
    public List<SoundEditGrid> Grids;
    public int WindowHeight;

    public List<SoundNote> Clipboard;

    public SoundEditSelectRect(AudioMixer audioMixer, Position parent, ReferenceValue<int> editingTrackIndex, List<SoundEditGrid> grids, SoundEditCursor cursor, int windowHeight)
    {
        SelectAreaRect = new Rect(0, windowHeight, 1);
        SelectAreaRect.Position.Parent = parent;
        SelectAreaRect.AddBlend = true;

        Clipboard = new List<SoundNote>();

        AudioMixer = audioMixer;
        EditingTrackIndex = editingTrackIndex;
        Grids = grids;
        Cursor = cursor;
        WindowHeight = windowHeight;
    }

    public void Update()
    {
        if (Input.Instance.MouseLeft.IsPush)
        {
            SelectAreaRect.SetWidth(0);
            return;
        }

        if (Input.Instance.F8.IsPushStartPure)
        {
            var track = AudioMixer.Tracks[EditingTrackIndex.Value];
            var notes = AudioMixer.Tracks[EditingTrackIndex.Value].Notes;
            foreach (var note in notes)
            {
                note.WaveVolume.Adsr = NClone.Clone.ObjectGraph(track.WaveVolume.Adsr);
                note.AttackWaveType = NClone.Clone.ObjectGraph(track.AttackWaveType);
                note.OverrideWaveType.Value = NClone.Clone.ObjectGraph(track.WaveType.Value);
                note.WaveVolume.Volume = NClone.Clone.ObjectGraph(track.WaveVolume.Volume);
            }
        }

        var beat = Cursor.GridX + Cursor.CurrentPage * SoundEditGrid.GridWidth;

        if (Input.Instance.MouseRight.IsPushStartPure || Input.Instance.Ctrl.IsPushStartPure)
        {
            var gridX = Cursor.GridX;
            SelectAreaRect.Position.Point.X = gridX * SoundEditNote.GridSize;
            SelectAreaRect.SetWidth(0);

            Start = beat;
            StartGridX = gridX;
        }

        if (Input.Instance.MouseRight.IsPush || Input.Instance.Ctrl.IsPush)
        {
            var gridX = Cursor.GridX + 1;
            var start = Start - Cursor.CurrentPage * SoundEditGrid.GridWidth;

            if (start < 0)
            {
                SelectAreaRect.Position.Point.X = 0;
                SelectAreaRect.SetWidth(gridX * SoundEditGrid.GridWidth);
            }
            else
            {
                SelectAreaRect.Position.Point.X = start * SoundEditNote.GridSize;
                SelectAreaRect.SetWidth(gridX * SoundEditNote.GridSize - SelectAreaRect.Position.Point.X);
            }

            End = beat;

            if (Start < End && Input.Instance.Down.IsPushStartPure)
            {
                ChangeMelody(-1);
            }
            else if (Start < End && Input.Instance.Up.IsPushStartPure)
            {
                ChangeMelody(1);
            }

            else if (Start < End && Input.Instance.U.IsPushStartPure)
            {
                ChangeKey(1);
            }

            else if (Start < End && Input.Instance.J.IsPushStartPure)
            {
                ChangeKey(-1);
            }
            else if (Start < End && Input.Instance.V.IsPushStartPure)
            {
                ChangeVolume();
            }
            else if (Start < End && Input.Instance.B.IsPushStartPure)
            {
                ChangeAdsr();
            }
        }

        if (Input.Instance.MouseRight.IsPushEnd || Input.Instance.Ctrl.IsPushEnd)
        {
            SelectAreaRect.SetWidth(0);
            End = beat;

            if (Start == End)
            {
                Paste();
            }
            else if (Start < End && Input.Instance.D.IsPush)
            {
                Remove();
            }
            else if (Start < End)
            {
                Copy();
            }
        }
    }

    public void Copy()
    {
        Offset = Start;
        Clipboard = AudioMixer.Tracks[EditingTrackIndex.Value].Notes
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
    }

    public void Remove()
    {
        Offset = Start;
        var notes = AudioMixer.Tracks[EditingTrackIndex.Value].Notes
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
        Clipboard = notes;
        foreach (var note in notes)
        {
            AudioMixer.Tracks[EditingTrackIndex.Value].Notes.Remove(note);
            Grids[EditingTrackIndex.Value].RemoveNote(note);
        }

        AudioUndoManager.Instance.Commit(
            new AudioUndoManager.RemoveNotes(EditingTrackIndex.Value, notes));
    }

    public void ChangeMelody(int add)
    {
        var notes = AudioMixer.Tracks[EditingTrackIndex.Value].Notes
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
        Clipboard = notes;
        foreach (var note in notes)
        {
            note.Melody.Value += add;
        }
    }

    public void ChangeKey(int add)
    {
        var firstKey = AudioMixer.Tracks.SelectMany(_ => _.Notes)
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList()
            .FirstOrDefault()?.AddKey.Value;

        if (firstKey == null)
        {
            return;
        }

        firstKey += add;

        foreach (var track in AudioMixer.Tracks)
        {
            var notes = track.Notes
                .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
            Clipboard = notes;
            foreach (var note in notes)
            {
                note.AddKey.Value = firstKey.Value;
            }
        }
    }

    public void ChangeVolume()
    {
        var track = AudioMixer.Tracks[EditingTrackIndex.Value];
        var notes = AudioMixer.Tracks[EditingTrackIndex.Value].Notes
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
        Clipboard = notes;
        foreach (var note in notes)
        {
            note.WaveVolume.Volume.Value = track.WaveVolume.Volume.Value;
        }
    }

    public void ChangeAdsr()
    {
        var track = AudioMixer.Tracks[EditingTrackIndex.Value];
        var notes = AudioMixer.Tracks[EditingTrackIndex.Value].Notes
            .Where(_ => _.BeatOffset.Value >= Start && _.BeatOffset.Value <= End).ToList();
        Clipboard = notes;
        foreach (var note in notes)
        {
            note.WaveVolume.Adsr = NClone.Clone.ObjectGraph(track.WaveVolume.Adsr);
            note.AttackWaveType = NClone.Clone.ObjectGraph(track.AttackWaveType);
            note.WaveVolume.Volume = NClone.Clone.ObjectGraph(track.WaveVolume.Volume);
            note.OverrideWaveType.Value = NClone.Clone.ObjectGraph(track.WaveType.Value);
        }
    }

    public void Paste()
    {
        var track = AudioMixer.Tracks[EditingTrackIndex.Value];
        var pastedNotes = new List<SoundNote>();
        foreach (var note in Clipboard)
        {
            var newNote = new SoundNote(
                new ReferenceValue<int>(note.BeatOffset.Value),
                new ReferenceValue<int>(note.Melody.Value),
                new ReferenceValue<int>(note.Length.Value),
                new ReferenceValue<int>(note.Add.Value),
                new ReferenceValue<int>(note.AddKey.Value),
                new ReferenceValue<bool>(note.Slide.Value),
                new ReferenceValue<int>(note.AttackWaveType.Value),
                new ReferenceValue<int>(note.OverrideWaveType.Value),
                Clone.ObjectGraph(note.WaveVolume));
            newNote.BeatOffset.Value = note.BeatOffset.Value - Offset + Start;
            track.AddNote(newNote);
            pastedNotes.Add(newNote);

            Grids[EditingTrackIndex.Value].AddNote(newNote);
        }

        if (pastedNotes.Count > 0)
        {
            AudioUndoManager.Instance.Commit(
                new AudioUndoManager.AddNotes(EditingTrackIndex.Value, pastedNotes));
        }
    }

    public void Draw()
    {
        SelectAreaRect.Draw();
    }
}