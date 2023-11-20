using System.Threading.Tasks;

using GreenStar.Persistence;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class PersistTurnTranscript : TurnTranscript
{
    private readonly IPersistence _persistence;

    public PersistTurnTranscript(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override async Task ExecuteAsync(Context context)
    {
        await _persistence.PersistFullAsync(context.TurnContext, context.PlayerContext, context.ActorContext);
    }
}