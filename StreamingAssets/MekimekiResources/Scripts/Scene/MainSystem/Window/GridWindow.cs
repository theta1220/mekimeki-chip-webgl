using System;
using System.Collections.Generic;
using Sirius.Engine;

public class GridWindow : WindowSystemBase
{
    public const int GridWindowSizeWidth = 384;
    public const int GridWindowSizeHeight = 280;
    public Position Position;
    public Position InnerPosition;
    public Position WindowSliderPosition;
    public Sprite GridSprite;
    public List<SoundEditGrid> Grids;
    public SoundEditCursor Cursor;
    public ReferenceValue<int> EditingTrackIndex;
    public AudioMixer AudioMixer;
    public SoundEditNoteInfoWindow NoteInfoWindow;
    public SoundEditSelectRect SelectRect;
    public List<Action> OnTrackChangedEvents;
    public SliceSprite WindowSlider;
    public Rect WindowSliderBackRect;
    public bool IsWindowSliding;
    public Point SliderToMouseOffset;

    public GridWindow(AudioMixer audioMixer) : base("editor", GridWindowSizeWidth + 4 + 11, GridWindowSizeHeight + 4,
        false)
    {
        Initialize(audioMixer);
    }

    public void Initialize(AudioMixer audioMixer)
    {
        AudioMixer = audioMixer;
        Position = new Position(Window.InnerPosition);
        InnerPosition = new Position(Position);
        Window.Position.Set(230, 40);
        WindowSliderPosition = new Position(Position);
        WindowSliderPosition.Set(GridWindowSizeWidth + 1, 0);
        WindowSlider = new SliceSprite("UI/common_button.png", 10, 100, false);
        WindowSlider.Position.Parent = WindowSliderPosition;
        WindowSliderBackRect = new Rect(WindowSlider.Width + 5, GridWindowSizeHeight, 2);
        WindowSliderBackRect.Position.Parent = WindowSliderPosition;
        WindowSliderBackRect.Position.Set(-1, 0);
            IsWindowSliding = false;
        EditingTrackIndex = new ReferenceValue<int>(0);
        GridSprite = new Sprite("SoundEdit/grid.png", false);
        GridSprite.Position.Parent = InnerPosition;
        GridSprite.SetClip(0, 0, GridWindowSizeWidth, GridWindowSizeHeight, Window.InnerPosition);
        Cursor = new SoundEditCursor(InnerPosition, GridWindowSizeWidth, GridSprite.Height);
        Grids = new List<SoundEditGrid>();
        for (var i = 0; i < audioMixer.Tracks.Count; i++)
        {
            var grid = new SoundEditGrid(AudioMixer, EditingTrackIndex, InnerPosition, Cursor, GridWindowSizeWidth,
                GridWindowSizeHeight, GridSprite.Height);
            grid.Root.Set(0, 0);
            Grids.Add(grid);
        }

        SelectRect =
            new SoundEditSelectRect(AudioMixer, InnerPosition, EditingTrackIndex, Grids, Cursor, GridSprite.Height);
        SelectRect.SelectAreaRect.SetClip(0, 0, GridWindowSizeWidth, GridWindowSizeHeight, Window.InnerPosition);
        NoteInfoWindow = new SoundEditNoteInfoWindow(AudioMixer, EditingTrackIndex, Cursor);
        ReplaceAllNotes();
        AudioUndoManager.Instance.SetAudioMixer(AudioMixer, Grids);
        OnTrackChangedEvents = new List<Action>();
    }

    public override void Update()
    {
        base.Update();
        var sliderRate = UpdateWindowSlider();
        if (IsWindowSliding)
        {
            UpdateGridPosition(sliderRate);
            return;
        }

        Grids[EditingTrackIndex.Value].Update();
        foreach (var grid in Grids)
        {
            grid.UpdateNotes();
        }

        SelectRect.Update();
        NoteInfoWindow.Update();

        if (Input.Instance.S.IsPushStartPure)
        {
            AudioUndoManager.Instance.Redo();
            ReplaceAllNotes();
        }

        if (Input.Instance.A.IsPushStartPure)
        {
            AudioUndoManager.Instance.Undo();
            ReplaceAllNotes();
        }
    }

    public float UpdateWindowSlider()
    {
        if (Input.Instance.MouseLeft.IsPushStartPure && WindowSlider.IsHitCursor())
        {
            IsWindowSliding = true;
            var mousePosX = Input.Instance.GetMousePositionX();
            var mousePosY = Input.Instance.GetMousePositionY();
            SliderToMouseOffset = new Point(mousePosX, mousePosY - WindowSlider.Position.Point.Y);
        }
        if (IsWindowSliding)
        {
            var mousePosY = Input.Instance.GetMousePositionY();
            var sliderPosY = mousePosY - SliderToMouseOffset.Y;
            if (sliderPosY < 0)
            {
                sliderPosY = 0;
            }

            if (sliderPosY > GridWindowSizeHeight - WindowSlider.RectHeight)
            {
                sliderPosY = GridWindowSizeHeight - WindowSlider.RectHeight;
            }
            WindowSlider.Position.Set(0, sliderPosY);
        }
        if (IsWindowSliding && !Input.Instance.MouseLeft.IsPush)
        {
            IsWindowSliding = false;
        }

        return (float)WindowSlider.Position.Point.Y / (GridWindowSizeHeight - WindowSlider.RectHeight);
    }

    public void UpdateGridPosition(float sliderRate)
    {
        var y = (int)(-GridSprite.Height * sliderRate / 2);
        InnerPosition.Set(0, y);
    }

    public override void UpdateBackground()
    {
        base.UpdateBackground();

        Cursor.Update();

        if (Input.Instance.Escape.IsPushStartPure || Input.Instance.Space.IsPushStartPure)
        {
            if (!AudioPlayer.Instance.IsPlaying)
            {
                if (Input.Instance.Escape.IsPushStartPure)
                {
                    Cursor.CurrentPageCursor = 0;
                }

                int bpm = AudioMixer.Bpm.Value;
                double sec = bpm / 60.0;
                int sampleNum = (int)(AudioPlayer.SampleRate / sec) * 2 *
                                (AudioPlayer.Instance.MonoralMode.Value ? 1 : 2);
                AudioPlayer.Instance.Play(
                    (ulong)(Cursor.CurrentPage * sampleNum), AudioMixer);
            }
            else
            {
                var sec = AudioPlayer.Instance.Count / 2.0 / AudioPlayer.SampleRate;
                var beat = (AudioMixer.Bpm.Value * (sec / 60.0) * 4); // 16分音符基準なので/4している
                var page = beat / SoundEditGrid.GridWidth;
                Cursor.CurrentPageCursor = (int)page;
                AudioPlayer.Instance.Stop();
            }
        }

        if (Input.Instance.E.IsPushStartPure)
        {
            AudioPlayer.Instance.Stop();
            EditingTrackIndex.Value++;
            if (AudioMixer.Tracks.Count <= EditingTrackIndex.Value)
            {
                EditingTrackIndex.Value = 0;
            }

            Grids[EditingTrackIndex.Value].PreviewSound();
            OnTrackChanged();
        }

        if (Input.Instance.Q.IsPushStartPure)
        {
            AudioPlayer.Instance.Stop();
            EditingTrackIndex.Value--;
            if (EditingTrackIndex.Value < 0)
            {
                EditingTrackIndex.Value = AudioMixer.Tracks.Count - 1;
            }

            Grids[EditingTrackIndex.Value].PreviewSound();
            OnTrackChanged();
        }
    }

    public void OnTrackChanged()
    {
        foreach (var onTrackChangedEvent in OnTrackChangedEvents)
        {
            onTrackChangedEvent.Invoke();
        }
    }

    public override bool Draw()
    {
        if (!base.Draw())
        {
            return false;
        }
        WindowSliderBackRect.Draw();
        GridSprite.Draw();
        int count = 0;
        foreach (var grid in Grids)
        {
            var active = count == EditingTrackIndex.Value;
            if (active)
            {
                count++;
                continue;
            }

            grid.SetActiveColor(true);
            grid.Draw();
            count++;
        }

        Grids[EditingTrackIndex.Value].SetActiveColor(false);
        Grids[EditingTrackIndex.Value].Draw();
        SelectRect.Draw();
        Cursor.Draw();
        WindowSlider.Draw();
        NoteInfoWindow.Draw();

        return true;
    }

    public void ReplaceAllNotes()
    {
        int count = 0;
        foreach (var grid in Grids)
        {
            grid.ReplaceAllNote(AudioMixer.Tracks[count].Notes);
            count++;
        }
    }
}