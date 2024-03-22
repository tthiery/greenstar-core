using GreenStar.Cartography.Builder;

namespace GreenStar.AppService.Setup;

public record GameType(string Name);
public record PersistedGame(Guid Id, string Type, Guid HumanPlayerId);

public interface ISetupService
{
    IEnumerable<GameType> GetGameTypes();
    IEnumerable<StellarType> GetStellarTypes();
    Task<IEnumerable<PersistedGame>> GetPersistedGamesAsync();
    Task<PersistedGame> CreateGameAsync(string selectedGameType, int nrOfAIPlayers, StellarType selectedStellarType);
    Task<PersistedGame> LoadGameAsync(PersistedGame game);
}
