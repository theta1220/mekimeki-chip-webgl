using System;
using System.Text.Json.Serialization;

[Serializable]
public class ReferenceValue<T>
{
    public T Value { get; set; }

    [JsonConstructor]
    public ReferenceValue(T value)
    {
        Value = value;
    }
}