using System;
using System.IO;
using System.Text.Json;

using GreenStar.Research;

namespace GreenStar.TurnEngine;

public class FileSystemTechnologyDefinitionLoader : ITechnologyDefinitionLoader
{
    private readonly string _rootDir;

    public FileSystemTechnologyDefinitionLoader(string rootDir)
    {
        _rootDir = rootDir;
    }

    public Technology[] GetTechnologiesByDefinition(string technologyDefinition)
    {
        var path = Path.Combine(_rootDir, "techtree-" + technologyDefinition + ".json");

        using var fileStream = File.Open(path, FileMode.Open, FileAccess.Read);

        var tree = JsonSerializer.Deserialize<Technology[]>(fileStream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("failed to load tree");

        fileStream.Close();

        return tree;
    }
}