namespace GreenStar.Persistence;

public interface IPersistenceWriter
{
    void Write<T>(string property, T value);
}
