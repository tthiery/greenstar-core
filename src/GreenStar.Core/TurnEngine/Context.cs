using GreenStar.TurnEngine;

namespace GreenStar;

public record Context(ITurnContext TurnContext, IPlayerContext PlayerContext, IActorContext ActorContext, Player? Player = null);
