
    using System.Collections.Generic;
    using System.Linq;
    using Sirius.Engine;

    public class AudioUndoManager : Singleton<AudioUndoManager>
    {
        public interface IUndoObject
        {
            void Undo();
            void Redo();
        }

        public class AddNotes : IUndoObject
        {
            public int TrackIndex;
            public List<SoundNote> Notes;

            public AddNotes(int trackIndex, SoundNote note)
            {
                TrackIndex = trackIndex;
                Notes = new List<SoundNote>() { note };
            }
            public AddNotes(int trackIndex, List<SoundNote> notes)
            {
                TrackIndex = trackIndex;
                Notes = notes;
            }

            public void Undo()
            {
                foreach (var note in Notes)
                {
                    AudioUndoManager.Instance.AudioMixer.Tracks[TrackIndex].RemoveNote(note);
                    AudioUndoManager.Instance.Grids[TrackIndex].RemoveNote(note);
                }
            }

            public void Redo()
            {
                foreach (var note in Notes)
                {
                    AudioUndoManager.Instance.AudioMixer.Tracks[TrackIndex].AddNote(note);
                    AudioUndoManager.Instance.Grids[TrackIndex].AddNote(note);
                }
            }
        }

        public class RemoveNotes : IUndoObject
        {
            public int TrackIndex;
            public List<SoundNote> Notes;

            public RemoveNotes(int trackIndex, SoundNote note)
            {
                TrackIndex = trackIndex;
                Notes = new List<SoundNote>() { note };
            }
            public RemoveNotes(int trackIndex, List<SoundNote> notes)
            {
                TrackIndex = trackIndex;
                Notes = notes;
            }

            public void Undo()
            {
                foreach (var note in Notes)
                {
                    AudioUndoManager.Instance.AudioMixer.Tracks[TrackIndex].AddNote(note);
                    AudioUndoManager.Instance.Grids[TrackIndex].AddNote(note);
                }

                var notes = AudioUndoManager.Instance.Grids[TrackIndex].Notes;
                foreach (var note in notes.ToArray())
                {
                    if (Notes.Contains(note.Note))
                    {
                        notes.Remove(note);
                    }
                }
            }

            public void Redo()
            {
                foreach (var note in Notes)
                {
                    AudioUndoManager.Instance.AudioMixer.Tracks[TrackIndex].RemoveNote(note);
                    AudioUndoManager.Instance.Grids[TrackIndex].RemoveNote(note);
                }

                var notes = AudioUndoManager.Instance.Grids[TrackIndex].Notes;
                foreach (var note in notes.ToArray())
                {
                    if (Notes.Contains(note.Note))
                    {
                        notes.Remove(note);
                    }
                }
            }
        }

        private readonly QueueStack<IUndoObject> _undoStack;
        private readonly QueueStack<IUndoObject> _redoStack;
        public AudioMixer AudioMixer;
        public List<SoundEditGrid> Grids;

        public AudioUndoManager()
        {
            _undoStack = new QueueStack<IUndoObject>();
            _redoStack = new QueueStack<IUndoObject>();
        }

        public void SetAudioMixer(AudioMixer audioMixer, List<SoundEditGrid> grids)
        {
            AudioMixer = audioMixer;
            Grids = grids;
            Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
            {
                return;
            }
            var obj = _undoStack.PopBack();
            _redoStack.PushBack(obj);
            obj.Undo();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
            {
                return;
            }
            var obj = _redoStack.PopBack();
            _undoStack.PushBack(obj);
            obj.Redo();
        }

        public void Commit(IUndoObject obj)
        {
            _undoStack.PushBack(obj);
            _redoStack.Clear();
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
