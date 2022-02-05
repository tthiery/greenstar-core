using System;

namespace GreenStar;

public enum CommandArgumentDataType
{
    LocatableAndHospitableReference,
    ActorReference,
}

public record CommandArgument(string Name, CommandArgumentDataType DataType, string Value);
public record Command(string Id, string Title, Guid ActorId, CommandArgument[] Arguments);
