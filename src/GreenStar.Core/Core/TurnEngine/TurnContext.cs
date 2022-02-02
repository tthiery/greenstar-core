namespace GreenStar.Core.TurnEngine;

public class TurnContext : ITurnContext, ITurnView
{
    public int Turn { get; set; } = 0;
}
