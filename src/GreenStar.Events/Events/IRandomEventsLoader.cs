using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenStar.Events;

public interface IRandomEventsLoader
{
    Task<IEnumerable<RandomEvent>> LoadRandomEventsAsync();
}
