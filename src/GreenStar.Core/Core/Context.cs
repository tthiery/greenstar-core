namespace GreenStar.Core
{
    public class Context
    {

        public Context(ITurnContext turnContext, IPlayerContext playerContext, IActorContext actorContext)
        {
            TurnContext = turnContext ?? throw new System.ArgumentNullException(nameof(turnContext));
            PlayerContext = playerContext ?? throw new System.ArgumentNullException(nameof(playerContext));
            ActorContext = actorContext ?? throw new System.ArgumentNullException(nameof(actorContext));
        }

        public ITurnContext TurnContext { get; }
        public IPlayerContext PlayerContext { get; }
        public IActorContext ActorContext { get; }
    }
}