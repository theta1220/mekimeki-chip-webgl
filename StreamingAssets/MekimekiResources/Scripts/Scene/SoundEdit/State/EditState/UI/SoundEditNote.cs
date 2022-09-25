using Sirius.Engine;

public class SoundEditNote
{
    public const int GridSize = 8;
    public Position Root;
    public Position Origin;
    public Position FacePosition;
    public Sprite NoteSprite;
    public Sprite LeftNoteSprite;
    public Sprite RightNoteSprite;
    public Sprite CenterNoteSprite;
    public Sprite OpenNoteSprite;
    public Sprite NoteFaceSprite;
    public Sprite NoteSlideFaceSprite;

    public SoundNote Note;

    public SoundEditNote(SoundEditGrid grid, SoundNote note, int gridSizeWidth, int gridSizeHeight)
    {
        Note = note;
        Root = new Position(grid.Root);
        Origin = new Position(Root);
        Origin.Set(-1, -1);
        FacePosition = new Position(Origin);
        FacePosition.Set(0, 1);
        NoteSprite = new Sprite("SoundEdit/note.png", true);
        NoteSprite.Position.Parent = Origin;
        LeftNoteSprite = new Sprite("SoundEdit/long_left.png", true);
        LeftNoteSprite.Position.Parent = Origin;
        CenterNoteSprite = new Sprite("SoundEdit/long_center.png", true);
        CenterNoteSprite.Position.Parent = Origin;
        RightNoteSprite = new Sprite("SoundEdit/long_right.png", true);
        RightNoteSprite.Position.Parent = Origin;
        OpenNoteSprite = new Sprite("SoundEdit/open.png", true);
        OpenNoteSprite.Position.Parent = Origin;
        NoteFaceSprite = new Sprite("SoundEdit/face.png", true);
        NoteFaceSprite.Position.Parent = FacePosition;
        NoteSlideFaceSprite = new Sprite("SoundEdit/face_slide.png", true);
        NoteSlideFaceSprite.Position.Parent = FacePosition;
        NoteSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        LeftNoteSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        CenterNoteSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        RightNoteSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        OpenNoteSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        NoteFaceSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
        NoteSlideFaceSprite.SetClip(0, 0, gridSizeWidth, gridSizeHeight, grid.Root.Parent.Parent);
    }

    public void Update()
    {
        NoteFaceSprite.Position.Set(0, Note.Add.Value * 2 + Note.AddKey.Value);
        NoteSlideFaceSprite.Position.Set(0, Note.Add.Value * 2 + Note.AddKey.Value);
        Root.Set(Root.Point.X, Note.Melody.Value * GridSize);
    }

    public void Draw()
    {
        if (Note.Length.Value == 1)
        {
            NoteSprite.Draw();
            if (Note.Slide.Value)
            {
                NoteSlideFaceSprite.Draw();
            }
            else
            {
                NoteFaceSprite.Draw();
            }
        }
        else
        {
            LeftNoteSprite.Draw();
            for (var i = 1; i <= Note.Length.Value - 2; i++)
            {
                CenterNoteSprite.Position.Point.X = GridSize * i;
                CenterNoteSprite.Draw();
            }

            RightNoteSprite.Position.Point.X = GridSize * (Note.Length.Value - 1);
            RightNoteSprite.Draw();
            
            if (Note.Slide.Value)
            {
                NoteSlideFaceSprite.Draw();
            }
            else
            {
                NoteFaceSprite.Draw();
            }
        }

        if (AudioPlayer.Instance.IsPlaying && Root.Point.X < GridSize)
        {
            OpenNoteSprite.Draw();
        }
    }

    public void SetAddColor(byte add)
    {
        NoteSprite.Add = add;
        LeftNoteSprite.Add = add;
        CenterNoteSprite.Add = add;
        RightNoteSprite.Add = add;
        OpenNoteSprite.Add = add;
        NoteFaceSprite.Add = add;
        NoteSlideFaceSprite.Add = add;
    }
}