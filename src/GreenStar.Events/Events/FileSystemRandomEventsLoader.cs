using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.FileProviders;

namespace GreenStar.Events;

public class FileSystemRandomEventsLoader : IRandomEventsLoader
{
    private readonly IFileProvider _fileProvider;
    private readonly string _rootDir;

    public FileSystemRandomEventsLoader(IFileProvider fileProvider, string rootDir)
    {
        _fileProvider = fileProvider;
        _rootDir = rootDir;
    }

    public async Task<IEnumerable<RandomEvent>> LoadRandomEventsAsync()
    {
        var path = Path.Combine(_rootDir, "random-events.json");

        using var fileStream = _fileProvider.GetFileInfo(path).CreateReadStream();

        var result = await JsonSerializer.DeserializeAsync<RandomEvent[]>(fileStream);

        fileStream.Close();

        return result ?? Array.Empty<RandomEvent>();
    }
}