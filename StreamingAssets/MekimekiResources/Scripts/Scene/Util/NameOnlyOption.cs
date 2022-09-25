public class NameOnlyOption : IMenuOption
{
    public string Name { get; }

    public object Reference
    {
        get { return null; }
        set {  }
    }

    public NameOnlyOption(string name)
    {
        Name = name;
    }

    public string BuildText()
    {
        return Name;
    }

    public void Invoke(int dir)
    {
    }
}