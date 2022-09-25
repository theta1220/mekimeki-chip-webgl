using System;

public class DoubleMenuOption : IMenuOption
{
    public string Name => _name;
    public ReferenceValue<double> Value { get; set; }
    public object Reference
    {
        get { return Value; }
        set { Value = value as ReferenceValue<double>; }
    }
    private string _name;
    private double _max, _min, _interval;
    private Action _action;

    public DoubleMenuOption(string name, ReferenceValue<double> value,
        double min, double max, double interval = 1, Action action = null)
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
        return $"{_name} <{(Value.Value * 100):000}>";
    }

    public void Invoke(int dir)
    {
        if (dir == -1)
        {
            Value.Value -= _interval;
            if (Value.Value - _interval <= _min)
            {
                Value.Value = _min;
            }
        }
        else if (dir == 1)
        {
            Value.Value += _interval;
            if (Value.Value + _interval > _max)
            {
                Value.Value = _max;
            }
        }

        if (_action != null)
        {
            _action.Invoke();
        }
    }
}