using System.Threading.Tasks;

using GreenStar.Research;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class ResearchSetup : SetupTranscript
{
    private ResearchProgressEngine _researchManager;
    private readonly IPlayerTechnologyStateLoader _playerTechnologyStateLoader;

    public ResearchSetup(ResearchProgressEngine researchManager, IPlayerTechnologyStateLoader playerTechnologyStateLoader)
    {
        _researchManager = researchManager;
        _playerTechnologyStateLoader = playerTechnologyStateLoader;
    }

    public override async Task ExecuteAsync(Context context)
    {
        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            var state = _researchManager.CreateTechnologyStateForPlayer("default");
            _playerTechnologyStateLoader.Save(player.Id, state);
        }
    }
}