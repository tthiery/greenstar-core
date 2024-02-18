using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using GreenStar.Research;

public class FileSystemPlayerTechnologyStateLoader : IPlayerTechnologyStateLoader
{
    private readonly Guid _gameId;
    private readonly string _gameType;

    public string FileNameFor(Guid playerId)
        => $"save_tech_{_gameType}_{_gameId}_{playerId}.json";

    public FileSystemPlayerTechnologyStateLoader(Guid gameId, string gameType)
    {
        _gameId = gameId;
        _gameType = gameType;
    }

    public async Task<PlayerTechnologyState> LoadAsync(Guid playerId)
    {
        using var fileStream = File.OpenRead(FileNameFor(playerId));

        var state = await JsonSerializer.DeserializeAsync<PlayerTechnologyState>(fileStream);

        fileStream?.Close();

        return state ?? throw new ArgumentException("Could not find state for given playerId", nameof(playerId));
    }

    public async Task SaveAsync(Guid playerId, PlayerTechnologyState state)
    {
        using var fileStream = new FileStream(FileNameFor(playerId), FileMode.Create, FileAccess.Write);

        await JsonSerializer.SerializeAsync<PlayerTechnologyState>(fileStream, state);

        fileStream.Flush();
        fileStream.Close();
    }
}