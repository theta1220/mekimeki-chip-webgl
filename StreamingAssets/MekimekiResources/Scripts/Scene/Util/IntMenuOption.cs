using System;
using Sirius.Engine;

public class IntMenuOption : IMenuOption
{
    public string Name => _name;
    public ReferenceValue<int> Value { get; set; }
    public object Reference
    {
        get { return Value; }
        set { Value = value as ReferenceValue<int>; }
    }
    private string _name;
    private int _max, _min, _interval;
    private Action _action;

    public IntMenuOption(string name, ReferenceValue<int> value, int min, int max, int interval = 1,
        Action action = null)
    {
        _name = name;
        Value = value;
        _max = max;
        _min = min;
        _interval = interval;
        _action = action;
    }

    public string BuildText()
    {
        return $"{_name} <{Value.Value:000}>";
    }

    public void Invoke(int dir)
    {
        if (dir == -1)
        {
            if (Value.Value - _interval >= _min)
            {
                Value.Value -= _interval;
            }
        }
        else if (dir == 1)
        {
            if (Value.Value + _interval <= _max)
            {
                Value.Value += _interval;
            }
        }
        
        if (_action != null)
        {
            _action.Invoke();
        }
    }
}