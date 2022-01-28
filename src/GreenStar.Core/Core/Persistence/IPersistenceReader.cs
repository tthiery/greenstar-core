using System.Collections.Generic;

namespace GreenStar.Core.Persistence
{
    public interface IPersistenceReader
    {
        T Read<T>(string property);
        IEnumerable<string> ReadPropertyNames(string? prefix = null);
    }
}