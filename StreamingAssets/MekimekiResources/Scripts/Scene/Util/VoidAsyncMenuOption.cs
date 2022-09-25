using System;
using Cysharp.Threading.Tasks;

public class VoidAsyncMenuOption<T> : IMenuOption
{
    public string Name { get; }
    public Func<T, UniTask> Func { get; }
    public T Arg { get; }
    public Func<UniTask> Callback { get; }
    public int Dummy;
    public object Reference
    {
        get { return null; }
        set { Dummy = (int)value; }
    }

    public VoidAsyncMenuOption(string name, Func<T, UniTask> func, T arg, Func<UniTask> callback = null)
    {
        Name = name;
        Func = func;
        Arg = arg;
        Callback = callback;
    }

    public string BuildText()
    {
        return Name;
    }

    public void Invoke(int dir)
    {
        if (dir == 0)
        {
            UniTask.Run(async () =>
            {
                await Func.Invoke(Arg);
                if (Callback != null)
                {
                    await Callback();
                }
            });
        }
    }
}