using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenStar.Events;

public class InMemoryRandomEventsLoader : IRandomEventsLoader
{
    private readonly IEnumerable<RandomEvent> _events;

    public InMemoryRandomEventsLoader(IEnumerable<RandomEvent> events)
    {
        _events = events;
    }

    public Task<IEnumerable<RandomEvent>> LoadRandomEventsAsync()
        => Task.FromResult(_events);
}
