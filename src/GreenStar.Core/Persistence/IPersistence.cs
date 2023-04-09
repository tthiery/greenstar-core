using System.Threading.Tasks;

namespace GreenStar.Persistence;

public interface IPersistence
{
    Task PersistFullAsync(ITurnContext turnContext, IPlayerContext playerContext, IActorContext actorContext);
}
