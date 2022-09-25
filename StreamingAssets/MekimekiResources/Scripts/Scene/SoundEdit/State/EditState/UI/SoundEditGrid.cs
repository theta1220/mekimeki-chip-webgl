using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NClone;
using Sirius.Engine;

public class SoundEditGrid
{
    public const int GridWidth = 8;
    public const int GridHeight = 40;

    public Position Root;
    public List<SoundEditNote> Notes;
    public SoundNote CurrentNote;
    public SoundEditNoteEffect NoteEffect;
    public AudioMixer AudioMixer;
    public ReferenceValue<int> EditingTrackIndex;
    public SoundEditCursor Cursor;
    public int GridWindowSizeWidth;
    public int GridWindowSizeHeight;
    public int GridSpriteHeight;

    private int _noteAdd;

    private readonly static int[] NoteTones = new[] { 0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5, 6 };

    public SoundEditGrid(
        AudioMixer mixer,
        ReferenceValue<int> editingTrackIndex, 
        Position parent,
        SoundEditCursor cursor,
        int gridWindowSizeWidth,
        int gridWindowSizeHeight,
        int gridSpriteHeight)
    {
        AudioMixer = mixer;
        EditingTrackIndex = editingTrackIndex;
        Cursor = cursor;
        Notes = new List<SoundEditNote>();
        Root = new Position(parent);
        NoteEffect = new SoundEditNoteEffect(Root);
        GridWindowSizeWidth = gridWindowSizeWidth;
        GridWindowSizeHeight = gridWindowSizeHeight;
        GridSpriteHeight = gridSpriteHeight;
    }

    public void Update()
    {
        // 再生中はMIDIキーボード入力できるんだぜ
        if (AudioPlayer.Instance.IsPlaying)
        {
            MidiKeyInput();
        }

        // マウスでの入力
        MouseInput();
    }

    public void MidiKeyInput()
    {
        if (Input.Instance.IsMidiKeyInputCount > 0)
        {
            var sec = AudioPlayer.Instance.Count / 2.0 / AudioPlayer.SampleRate;
            var playingBeat = (AudioMixer.Bpm.Value * (sec / 60.0) * 4);
            var tone = NoteTones[Input.Instance.MidiKeyNote % NoteTones.Length];
            var octave = Input.Instance.MidiKeyNote / NoteTones.Length;
            var melody = tone + 7 * octave;

            if (CurrentNote == null || (CurrentNote != null && CurrentNote.Melody.Value != melody))
            {
                var newNote = new SoundNote(
                    new ReferenceValue<int>((int)Math.Round(playingBeat)),
                    new ReferenceValue<int>(tone + 7 * octave),
                    new ReferenceValue<int>(1),
                    new ReferenceValue<int>(0),
                    new ReferenceValue<int>(0),
                    new ReferenceValue<bool>(false),
                    new ReferenceValue<int>(
                        AudioMixer.Tracks[EditingTrackIndex.Value].AttackWaveType.Value),
                    new ReferenceValue<int>(-1),
                    Clone.ObjectGraph(AudioMixer.Tracks[EditingTrackIndex.Value].WaveVolume));

                newNote.WaveVolume.Volume.Value = Input.Instance.MidiKeyVelocity;
                AudioMixer.Tracks[EditingTrackIndex.Value].AddNote(newNote);

                AddNote(newNote);
                // PreviewSound(newNote);

                CurrentNote = newNote;
                AudioUndoManager.Instance.Commit(
                    new AudioUndoManager.AddNotes(EditingTrackIndex.Value, newNote));
            }
            else
            {
                var len = (int)playingBeat - CurrentNote.BeatOffset.Value;
                if (len > 0)
                {
                    CurrentNote.Length.Value = len;
                }
            }
        }
        else
        {
            CurrentNote = null;
        }
    }

    public void MouseInput()
    {
        var beat = Cursor.GridX + Cursor.CurrentPage * GridWidth;
        if (Input.Instance.MouseLeft.IsPushStartPure &&
            Cursor.GridX >= 0)
        {
            if (AudioPlayer.Instance.IsPlaying)
            {
                AudioPlayer.Instance.Stop();
                return;
            }

            var newNote = new SoundNote(
                new ReferenceValue<int>(beat),
                new ReferenceValue<int>(Cursor.GridY),
                new ReferenceValue<int>(1),
                new ReferenceValue<int>(0),
                new ReferenceValue<int>(0),
                new ReferenceValue<bool>(false),
                new ReferenceValue<int>(
                    AudioMixer.Tracks[EditingTrackIndex.Value].AttackWaveType.Value),
                new ReferenceValue<int>(-1),
                Clone.ObjectGraph(AudioMixer.Tracks[EditingTrackIndex.Value].WaveVolume));

            if (!RemoveNotes(newNote.BeatOffset.Value, newNote.Melody.Value))
            {
                AudioMixer.Tracks[EditingTrackIndex.Value].AddNote(newNote);

                AddNote(newNote);
                PreviewSound(newNote);

                CurrentNote = newNote;
                AudioUndoManager.Instance.Commit(
                    new AudioUndoManager.AddNotes(EditingTrackIndex.Value, newNote));
            }
            else
            {
                CurrentNote = null;
            }
        }

        if (Input.Instance.MouseLeft.IsPush && Cursor.GridX >= 0)
        {
            if (Cursor.CurrentPage >= 0)
            {
                if (CurrentNote != null && CurrentNote.BeatOffset.Value < beat + 1)
                {
                    CurrentNote.Length.Value = beat - CurrentNote.BeatOffset.Value + 1;
                }

                if (CurrentNote != null)
                {
                    var diffY = Cursor.GridY - CurrentNote.Melody.Value;
                    var diffX = Cursor.GridX - CurrentNote.BeatOffset.Value;
                    var addThreshold = 1;
                    if (diffY > addThreshold)
                    {
                        CurrentNote.Add.Value = 1;
                    }
                    else if (diffY < -addThreshold)
                    {
                        CurrentNote.Add.Value = -1;
                    }
                    else
                    {
                        CurrentNote.Add.Value = 0;
                    }

                    if (Input.Instance.MouseRight.IsPushStartPure)
                    {
                        CurrentNote.Slide.Value = !CurrentNote.Slide.Value;
                    }

                    if (CurrentNote.Add.Value != _noteAdd)
                    {
                        _noteAdd = CurrentNote.Add.Value;
                        PreviewSound(CurrentNote);
                    }
                }
            }
        }
    }

    public void UpdateNotes()
    {
        foreach (var note in Notes)
        {
            note.Update();
        }
    }

    public void AddNote(SoundNote note)
    {
        if (Notes.Select(_ => _.Note).ToList()
            .Find(_ => _.BeatOffset == note.BeatOffset && _.Melody == note.Melody) != null)
        {
            return;
        }

        var newEditNote = new SoundEditNote(this, note, GridWindowSizeWidth, GridWindowSizeHeight);
        var x = note.BeatOffset.Value % GridWidth;
        var y = note.Melody.Value;
        newEditNote.Root.Point.X = x * SoundEditNote.GridSize;
        newEditNote.Root.Point.Y = y * SoundEditNote.GridSize;
        Notes.Add(newEditNote);

        Notes.Sort(
            (a, b) =>
            {
                if (a.Note.BeatOffset == b.Note.BeatOffset)
                {
                    return a.Note.Melody.Value - b.Note.Melody.Value;
                }
                else
                {
                    return a.Note.BeatOffset.Value - b.Note.BeatOffset.Value;
                }
            });
    }

    public void PreviewSound(SoundNote note = null)
    {
        var current = AudioMixer;
        var currentTrack = current.Tracks[EditingTrackIndex.Value];
        var previewMixer = new AudioMixer(null, null, null, null, null);
        previewMixer.Format(current.Bpm.Value, current.Key.Value);
        previewMixer.MasterVolume.Value = AudioMixer.MasterVolume.Value;

        var previewNote = new SoundNote(
            new ReferenceValue<int>(0),
            new ReferenceValue<int>(note == null ? Cursor.GridY : note.Melody.Value),
            new ReferenceValue<int>(1),
            new ReferenceValue<int>(note == null ? 0 : note.Add.Value),
            new ReferenceValue<int>(note == null ? 0 : note.AddKey.Value),
            new ReferenceValue<bool>(false),
            new ReferenceValue<int>(note == null ? (int)WaveType.None : note.AttackWaveType.Value),
            new ReferenceValue<int>(note == null ? (int)WaveType.None : note.OverrideWaveType.Value),
            Clone.ObjectGraph(currentTrack.WaveVolume));

        var previewTrack = previewMixer.Tracks[EditingTrackIndex.Value];
        previewTrack.WaveVolume = Clone.ObjectGraph(currentTrack.WaveVolume);
        previewTrack.Volume.Value = currentTrack.Volume.Value;
        previewTrack.Octave = Clone.ObjectGraph(currentTrack.Octave);
        previewTrack.WaveType = Clone.ObjectGraph(currentTrack.WaveType);
        previewTrack.AudioChannelType = Clone.ObjectGraph(currentTrack.AudioChannelType);
        previewTrack.Pitch = Clone.ObjectGraph(currentTrack.Pitch);
        previewTrack.AddNote(previewNote);
        AudioPlayer.Instance.PlayPreview(previewMixer);
    }

    public void ReplaceAllNote(List<SoundNote> notes)
    {
        Notes.Clear();
        foreach (var note in notes)
        {
            AddNote(note);
        }
    }

    private bool RemoveNotes(int beat, int melody)
    {
        var isRemoved = false;
        var removeNotes =
            AudioMixer.Tracks[EditingTrackIndex.Value]
                .RemoveNote(beat, melody);

        foreach (var removeNote in removeNotes)
        {
            foreach (var editNote in Notes.ToArray())
            {
                if (removeNote == editNote.Note)
                {
                    Notes.Remove(editNote);
                    isRemoved = true;
                }
            }
        }

        if (removeNotes.Count > 0)
        {
            AudioUndoManager.Instance.Commit(
                new AudioUndoManager.RemoveNotes(EditingTrackIndex.Value, removeNotes));
        }

        return isRemoved;
    }

    public void RemoveNote(SoundNote targetNote)
    {
        foreach (var note in Notes.ToArray())
        {
            if (note.Note == targetNote)
            {
                Notes.Remove(note);
                break;
            }
        }
    }

    public void SetActiveColor(bool active)
    {
        var add = (byte)(active ? 1 : 0);
        foreach (var note in Notes)
        {
            note.SetAddColor(add);
        }
    }

    public void Draw()
    {
        if (AudioPlayer.Instance.IsPlaying)
        {
            foreach (var soundEditNote in Notes)
            {
                var sec = AudioPlayer.Instance.Count / 2.0 / AudioPlayer.SampleRate;
                var beat = (AudioMixer.Bpm.Value * (sec / 60.0) * 4); // 16分音符基準なので/4している
                soundEditNote.Root.Point.X = (int)(
                    soundEditNote.Note.BeatOffset.Value * SoundEditNote.GridSize
                    - beat * SoundEditNote.GridSize);

                if (soundEditNote.Root.Point.X + soundEditNote.Note.Length.Value * SoundEditNote.GridSize >= 0 &&
                    soundEditNote.Root.Point.X < GridWidth * SoundEditNote.GridSize * 6)
                {
                    soundEditNote.Draw();
                }
            }
        }
        else
        {
            foreach (var soundEditNote in Notes)
            {
                // ページにもとづいて座標計算
                soundEditNote.Root.Point.X =
                    soundEditNote.Note.BeatOffset.Value * SoundEditNote.GridSize
                    - Cursor.CurrentPage * GridWidth * SoundEditNote.GridSize;

                if (soundEditNote.Root.Point.X + soundEditNote.Note.Length.Value * SoundEditNote.GridSize >= 0 &&
                    soundEditNote.Root.Point.X < GridWidth * SoundEditNote.GridSize * 6)
                {
                    soundEditNote.Draw();
                }
            }
        }
    }
}