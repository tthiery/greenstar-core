using System.Threading.Tasks;

using GreenStar.Research;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class TechnologySetup : SetupTranscript
{
    private readonly TechnologyProgressEngine _technologyManager;
    private readonly IPlayerTechnologyStateLoader _playerTechnologyStateLoader;

    public TechnologySetup(TechnologyProgressEngine technologyManager, IPlayerTechnologyStateLoader playerTechnologyStateLoader)
    {
        _technologyManager = technologyManager;
        _playerTechnologyStateLoader = playerTechnologyStateLoader;
    }

    public override async Task ExecuteAsync(Context context)
    {
        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            var state = _technologyManager.CreateTechnologyStateForPlayer("default");
            await _playerTechnologyStateLoader.SaveAsync(player.Id, state);
        }
    }
}