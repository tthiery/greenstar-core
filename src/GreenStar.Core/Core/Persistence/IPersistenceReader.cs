namespace GreenStar.Core.Persistence
{
    public interface IPersistenceReader
    {
        T Read<T>(string property);
    }
}