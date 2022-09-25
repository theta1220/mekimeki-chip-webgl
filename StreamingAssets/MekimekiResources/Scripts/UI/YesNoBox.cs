using Cysharp.Threading.Tasks;
using Sirius.Engine;

public class YesNoBox : Singleton<YesNoBox> ,IDrawable
{
    public int Sorting => 1;
    public Sprite FrameSprite;
    public Text Text;
    public string YesText;
    public string NoText;
    public bool Answer;

    public YesNoBox()
    {
        var text = MasterDataLoader.Instance.Load("CommonText");
        YesText = text["yes"]["Ja"];
        NoText = text["no"]["Ja"];
        Answer = true;

        FrameSprite = new Sprite("UI/yesno_window.png", true);
        FrameSprite.Position.Set(
            ScreenHandler.Width - FrameSprite.Width - 12,
            ScreenHandler.Height - FrameSprite.Height - 32);
        Text = new Text(0);
        Text.Position.Parent = FrameSprite.Position;
        Text.Position.Set(6, 6);
    }

    public async UniTask<bool> Select()
    {
        Answer = true;
        DrawableManager.Instance.Add(this);
        while (Input.Instance.AnyButtonIsPush)
        {
            await UniTask.Yield();
        }

        while (!Input.Instance.A.IsPushEnd)
        {
            if (Input.Instance.Down.IsPushStart || Input.Instance.Up.IsPushStart)
            {
                Answer = !Answer;
            }

            await UniTask.Yield();
        }
        DrawableManager.Instance.Remove(this);
        return Answer;
    }

    public string BuildText()
    {
        if (Answer)
        {
            return $"> {YesText}\n {NoText}";
        }
        else
        {
            return $" {YesText}\n> {NoText}";
        }
    }

    public void Draw()
    {
        Text.SetText(BuildText());

        FrameSprite.Draw();
        Text.Draw();
    }
}