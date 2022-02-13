using System.Threading.Tasks;

using GreenStar.TurnEngine;

namespace GreenStar.Research;

public class AdjustResearchBudgetTurnTranscript : TurnTranscript
{
    private readonly IPlayerTechnologyStateLoader _stateLoader;
    private readonly ResearchProgressEngine _researchProgressEngine;

    public AdjustResearchBudgetTurnTranscript(IPlayerTechnologyStateLoader stateLoader, ResearchProgressEngine researchProgressEngine)
    {
        _stateLoader = stateLoader ?? throw new System.ArgumentNullException(nameof(stateLoader));
        _researchProgressEngine = researchProgressEngine ?? throw new System.ArgumentNullException(nameof(researchProgressEngine));
    }

    public override async Task ExecuteAsync(Context context)
    {
        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            var state = await _stateLoader.LoadAsync(player.Id);

            // Readjust budget since ...
            // ... initialize budget distribution
            // ... technologies might have maxed out meanwhile
            state = _researchProgressEngine.AdjustBudgetAndDetermineThresholds(state);

            await _stateLoader.SaveAsync(player.Id, state);
        }

    }
}