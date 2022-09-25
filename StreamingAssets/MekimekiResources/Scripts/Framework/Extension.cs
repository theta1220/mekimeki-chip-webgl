using System.Runtime.Serialization.Formatters.Binary;

public static class Extension
{
    public static T DeepClone<T>(T obj)
    {
        using (var memoryStream = new System.IO.MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj); // シリアライズ
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
        }
    }
}