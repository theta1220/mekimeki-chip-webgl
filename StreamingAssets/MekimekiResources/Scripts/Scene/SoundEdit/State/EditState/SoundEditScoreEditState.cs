using System.Collections.Generic;
using Sirius.Engine;

public class SoundEditScoreEditState : StateBase
{
    public SoundEditScene Scene;
    public Position Root;
    public List<SoundEditGrid> Grids;
    public ReferenceValue<int> EditingTrackIndex;
    public SrSprite GridSprite;
    public SoundEditCursor Cursor;
    public SoundEditMenuList MenuList;
    public SrSprite BackGround;
    public SrSprite FrameSprite;
    public SoundEditSelectRect SelectRect;
    public SoundEditSettingMenu SettingMenu;
    public SoundEditNoteInfoWindow NoteInfoWindow;

    public SoundEditScoreEditState(SoundEditScene scene)
    {
        Scene = scene;
        EditingTrackIndex = new ReferenceValue<int>(0);
    }

    public void Save()
    {
        FileManager.Instance.SaveToJsonWithPanel(Scene.AudioMixer, "json");
    }

    public void Load()
    {
        Scene.Load();
    }

    public void Export(bool isLoopMode)
    {
        WaveExporter.Export(Scene.AudioMixer, isLoopMode);
    }

    public override void OnEnter()
    {
        Root = new Position();
        Root.Set(97, 0);
        Cursor = new SoundEditCursor(Root, Screen.Width, Screen.Height);
        GridSprite = new SrSprite();
        GridSprite.Image = ResourceManager.Instance.LoadFromJson<Image>("SoundEdit/grid.json");
        GridSprite.Position.Parent = Root;
        Grids = new List<SoundEditGrid>();
        for (var i = 0; i < Scene.AudioMixer.Tracks.Count; i++)
        {
            var grid = new SoundEditGrid(Scene.AudioMixer, EditingTrackIndex, Root, Cursor, 384, 224, 224 * 2);
            grid.Root.Set(97, 0);
            Grids.Add(grid);
        }

        EditingTrackIndex.Value = 0;
        BackGround = new SrSprite();
        BackGround.Image = ResourceManager.Instance
            .LoadFromJson<Image>("SoundEdit/back_ground.json");
        FrameSprite = new SrSprite();
        FrameSprite.Image = ResourceManager.Instance
            .LoadFromJson<Image>("SoundEdit/screen_frame.json");
        FrameSprite.Position.Set(80, 0);
        MenuList = new SoundEditMenuList();
        MenuList.AddButton("Export", () => Export(false));
        MenuList.AddButton("Export LC", () => Export(true));
        MenuList.AddButton("Load", Load);
        MenuList.AddButton("Save", Save);
        MenuList.Root.Set(0, 35);
        SelectRect = new SoundEditSelectRect(Scene.AudioMixer, Root, EditingTrackIndex, Grids, Cursor, Screen.Height);
        SettingMenu = new SoundEditSettingMenu(Scene.AudioMixer, EditingTrackIndex);
        NoteInfoWindow = new SoundEditNoteInfoWindow(Scene.AudioMixer, EditingTrackIndex, Cursor);

        ReplaceAllNotes();
    }

    public override void Update()
    {
        SettingMenu.Update();
        Cursor.Update();
        Grids[EditingTrackIndex.Value].Update();
        foreach (var grid in Grids)
        {
            grid.UpdateNotes();
        }

        // Grid[EditingTrackIndex].NoteEffect.Update();
        MenuList.Update();
        SelectRect.Update();
        NoteInfoWindow.Update();

        if (Input.Instance.Escape.IsPushStartPure || Input.Instance.Space.IsPushStartPure)
        {
            if (!AudioPlayer.Instance.IsPlaying)
            {
                if (Input.Instance.Escape.IsPushStartPure)
                {
                    Cursor.CurrentPageCursor = 0;
                }

                int bpm = Scene.AudioMixer.Bpm.Value;
                double sec = bpm / 60.0;
                int sampleNum = (int)(AudioPlayer.SampleRate / sec) * 2 *
                                (AudioPlayer.Instance.MonoralMode.Value ? 1 : 2);
                AudioPlayer.Instance.Play(
                    (ulong)(Cursor.CurrentPage * sampleNum), Scene.AudioMixer);
            }
            else
            {
                var sec = AudioPlayer.Instance.Count / 2.0 / AudioPlayer.SampleRate;
                var beat = (Scene.AudioMixer.Bpm.Value * (sec / 60.0) * 4); // 16分音符基準なので/4している
                var page = beat / SoundEditGrid.GridWidth;
                Cursor.CurrentPageCursor = (int)page;
                AudioPlayer.Instance.Stop();
            }
        }

        if (Input.Instance.E.IsPushStartPure)
        {
            AudioPlayer.Instance.Stop();
            EditingTrackIndex.Value++;
            if (Scene.AudioMixer.Tracks.Count <= EditingTrackIndex.Value)
            {
                EditingTrackIndex.Value = 0;
            }

            Grids[EditingTrackIndex.Value].PreviewSound();
        }

        if (Input.Instance.Q.IsPushStartPure)
        {
            AudioPlayer.Instance.Stop();
            EditingTrackIndex.Value--;
            if (EditingTrackIndex.Value < 0)
            {
                EditingTrackIndex.Value = Scene.AudioMixer.Tracks.Count - 1;
            }

            Grids[EditingTrackIndex.Value].PreviewSound();
        }

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

        if (Input.Instance.Enter.IsPushEnd)
        {
            Scene.StateMachine.Switch<SoundEditSettingState>();
        }
    }

    public void ReplaceAllNotes()
    {
        int count = 0;
        foreach (var grid in Grids)
        {
            grid.ReplaceAllNote(Scene.AudioMixer.Tracks[count].Notes);
            count++;
        }
    }

    public override void Draw()
    {
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
        // Grid[EditingTrackIndex].NoteEffect.Draw();
        SelectRect.Draw();
        Cursor.Draw();
        NoteInfoWindow.Draw();
        MenuList.Draw();
        SettingMenu.Draw();
    }
}