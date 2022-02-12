using System.Threading.Tasks;

using GreenStar;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public class FreeShipBlueprintEvent : EventTranscript
{
    public FreeShipBlueprintEvent(string text, string[] arguments)
    {
    }

    public override Task ExecuteAsync(Context context)
    {
        throw new System.NotImplementedException();
    }
}