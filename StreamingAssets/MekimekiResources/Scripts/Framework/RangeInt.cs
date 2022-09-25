public class RangeInt
{
    private int _value;
    public int Min, Max;

    public RangeInt(int value, int min, int max)
    {
        _value = value;
        Min = min;
        Max = max;
    }

    public int Value
    {
        get { return _value; }
        set
        {
            _value = value;
            if (_value < Min)
            {
                _value = Max - 1;
            }

            if (_value >= Max)
            {
                _value = Min;
            }
        }
    }

    public void Increment()
    {
        Value++;
    }

    public void Decrement()
    {
        Value--;
    }
}