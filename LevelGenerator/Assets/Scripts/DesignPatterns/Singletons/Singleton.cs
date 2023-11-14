/// <summary>
/// A generic class for creating singleton instances of other classes.
/// </summary>

public class Singleton<T> where T : class, new()
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            _instance ??= new T();
            return _instance;
        }
    }
}
