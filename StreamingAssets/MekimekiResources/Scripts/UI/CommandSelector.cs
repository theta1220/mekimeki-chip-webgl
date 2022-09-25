using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirius.Engine;

public class CommandSelector : CommonTextBox
{
    public List<NameOnlyOption> Options;
    public RangeInt SelectIndex;

    public CommandSelector(int sorting) : base(sorting)
    {
        SelectIndex = new RangeInt(0, 0, 0);
        Options = new List<NameOnlyOption>();
    }

    public string BuildText()
    {
        var buf = "";
        var count = 0;
        foreach (var option in Options)
        {
            buf += (SelectIndex.Value == count ? "> " : "  ") + option.Name;

            if (count + 1 < Options.Count)
            {
                buf += "\n";
            }

            count++;
        }

        return buf;
    }

    public async UniTask<int> Select()
    {
        if (Options.Count <= SelectIndex.Value)
        {
            SelectIndex.Value = 0;
        }

        SelectIndex.Max = Options.Count;
        DrawableManager.Instance.Add(this);
        while (true)
        {
            Text.SetText(BuildText());
            SetFrame();

            if (Input.Instance.Down.IsPushStart)
            {
                SelectIndex.Increment();
            }

            if (Input.Instance.Up.IsPushStart)
            {
                SelectIndex.Decrement();
            }

            if (Input.Instance.A.IsPushEnd)
            {
                return SelectIndex.Value;
            }

            if (Input.Instance.B.IsPushEnd)
            {
                return -1;
            }

            await UniTask.Yield();
        }
    }

    public void Hide()
    {
        DrawableManager.Instance.Remove(this);
    }
}