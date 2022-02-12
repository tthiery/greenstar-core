using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreenStar.Events;

public class FileSystemRandomEventsLoader : IRandomEventsLoader
{
    private readonly string _rootDir;

    public FileSystemRandomEventsLoader(string rootDir)
    {
        _rootDir = rootDir;
    }

    public async Task<IEnumerable<RandomEvent>> LoadRandomEventsAsync()
    {
        var path = Path.Combine(_rootDir, "random-events.json");

        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

        var result = await JsonSerializer.DeserializeAsync<RandomEvent[]>(fileStream);

        fileStream.Close();

        return result ?? Array.Empty<RandomEvent>();
    }
}