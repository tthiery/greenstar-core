using System.Collections.Generic;

namespace GreenStar.Persistence;

public interface IPersistenceReader
{
    T Read<T>(string property);
    IEnumerable<string> ReadPropertyNames(string? prefix = null);
}
