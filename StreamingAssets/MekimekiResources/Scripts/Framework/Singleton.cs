public class Singleton<T> where T : class, new()
{
    public static T Instance = new T();
}