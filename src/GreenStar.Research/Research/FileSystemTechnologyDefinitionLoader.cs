using System;
using System.IO;
using System.Text.Json;

using Microsoft.Extensions.FileProviders;

namespace GreenStar.Research;

public class FileSystemTechnologyDefinitionLoader : ITechnologyDefinitionLoader
{
    private readonly IFileProvider _fileProvider;
    private readonly string _rootDir;

    public FileSystemTechnologyDefinitionLoader(IFileProvider fileProvider, string rootDir)
    {
        _fileProvider = fileProvider;
        _rootDir = rootDir;
    }

    public Technology[] GetTechnologiesByDefinition(string technologyDefinition)
    {
        var path = Path.Combine(_rootDir, "techtree-" + technologyDefinition + ".json");

        using var fileStream = _fileProvider.GetFileInfo(path).CreateReadStream();

        var tree = JsonSerializer.Deserialize<Technology[]>(fileStream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("failed to load tree");

        fileStream.Close();

        return tree;
    }
}
