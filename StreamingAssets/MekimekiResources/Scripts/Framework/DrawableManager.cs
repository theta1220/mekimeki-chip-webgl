using System.Collections.Generic;

public class DrawableManager : Singleton<DrawableManager>
{
    public List<IDrawable> Drawables;

    public DrawableManager()
    {
        Drawables = new List<IDrawable>();
    }
    
    public void Draw()
    {
        Drawables.Sort((a, b) => a.Sorting - b.Sorting);
        foreach (var drawable in Drawables)
        {
            drawable.Draw();
        }
    }

    public void Add(IDrawable target)
    {
        if (Drawables.Contains(target))
        {
            return;
        }
        Drawables.Add(target);
    }

    public void Remove(IDrawable target)
    {
        if (!Drawables.Contains(target))
        {
            return;
        }
        Drawables.Remove(target);
    }
}