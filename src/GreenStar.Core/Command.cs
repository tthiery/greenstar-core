using System;

namespace GreenStar;

public abstract record Command(string Id, string Title, Guid ActorId, string[] Arguments);
