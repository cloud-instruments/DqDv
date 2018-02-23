namespace WebApp.Interfaces
{
    public interface ICacheProvider
    {
        void Set(string key, object value);
        object Get(string key);
        void Remove(string key);
        void Clear();
    }
}
