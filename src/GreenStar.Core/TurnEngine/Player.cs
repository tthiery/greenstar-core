using System;

using GreenStar.Traits;

namespace GreenStar.TurnEngine;

public abstract class Player
{
    public Guid Id { get; }
    public int CompletedTurn { get; set; } = -1;

    // Traits
    public IdealConditions IdealConditions { get; set; } = new IdealConditions();
    public Relatable Relatable { get; set; } = new Relatable();
    public Resourceful Resourceful { get; set; } = new Resourceful();
    public Capable Capable { get; set; } = new Capable(Array.Empty<string>());

    public Player(Guid id)
    {
        Id = id;
    }
}
