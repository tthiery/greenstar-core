using System.Threading.Tasks;

namespace GreenStar.Persistence;

public interface IPersistence
{
    Task PersistActorsAsync(IActorContext actorContext);
}
