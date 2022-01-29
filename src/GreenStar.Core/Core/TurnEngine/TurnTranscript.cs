using System.Collections.Generic;

namespace GreenStar.Core.TurnEngine;

public abstract class TurnTranscript
{
    public abstract void Execute(Context context);

    public Dictionary<string, object> IntermediateData { get; set; } = new Dictionary<string, object>();
}
