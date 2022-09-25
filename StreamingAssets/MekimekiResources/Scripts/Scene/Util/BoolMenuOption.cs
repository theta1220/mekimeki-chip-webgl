using System;

public class BoolMenuOption : IMenuOption
{
    public string Name => _name;
    public ReferenceValue<bool> Value;
    public object Reference
    {
        get { return Value; }
        set { Value = value as ReferenceValue<bool>; }
    }
    private string _name;
    private Action _action;
    private const string TrueText = " ON";
    private const string FalseText = "OFF";

    public BoolMenuOption(string name, ReferenceValue<bool> value, Action action = null)
    {
        _name = name;
        Value = value;
        _action = action;
    }

    public string BuildText()
    {
        return $"{_name} <{(Value.Value ? TrueText: FalseText)}>";
    }

    public void Invoke(int dir)
    {
        Value.Value = !Value.Value;
        if (_action != null)
        {
            _action.Invoke();
        }
    }
}