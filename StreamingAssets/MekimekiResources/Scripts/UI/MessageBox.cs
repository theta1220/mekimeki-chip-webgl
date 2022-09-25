using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirius.Engine;

public class MessageBox : Singleton<MessageBox>, IDrawable
{
    public int Sorting => 0;
    public Sprite TextFrame;
    public Text Text;

    public MessageBox()
    {
        TextFrame = new Sprite("UI/message_window.png", true);
        Text = new Text(0);
        
        TextFrame.Position.Set(
            ScreenHandler.Width - TextFrame.Width - 6,
            ScreenHandler.Height - TextFrame.Height - 6);
        Text.Position.Parent = TextFrame.Position;
        Text.Position.Set(8,8);
    }

    public async UniTask Show(string message)
    {
        DrawableManager.Instance.Add(this);
        var buf = "";

        foreach (var c in message)
        {
            if (c == '▼')
            {
                buf += c;
                Text.SetText(buf);

                while (true)
                {
                    if (Input.Instance.A.IsPushEnd)
                    {
                        buf = "";
                        break;
                    }
                    await UniTask.Yield();
                }

                continue;
            }

            buf += c;
            Text.SetText(buf);
            await UniTask.Yield();
        }
    }

    public void Hide()
    {
        Text.SetText("");
        DrawableManager.Instance.Remove(this);
    }

    public void Draw()
    {
        TextFrame.Draw();
        Text.Draw();
    }
}