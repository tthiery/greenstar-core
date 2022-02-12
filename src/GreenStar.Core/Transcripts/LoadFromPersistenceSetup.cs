using System.Threading.Tasks;

using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class LoadFromPersistenceSetup : SetupTranscript
{
    public override Task ExecuteAsync(Context context)
    {
        return Task.CompletedTask;
    }
}