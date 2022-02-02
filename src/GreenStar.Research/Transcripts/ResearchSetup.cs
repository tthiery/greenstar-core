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

    public override void Execute(Context context)
    {
        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            var state = _researchManager.CreateTechnologyStateForPlayer("default");
            _playerTechnologyStateLoader.Save(player.Id, state);
        }
    }
}