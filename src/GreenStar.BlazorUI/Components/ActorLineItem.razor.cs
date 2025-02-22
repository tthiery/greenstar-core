using System.Numerics;

using GreenStar.AppService;
using GreenStar.AppService.Actor;
using GreenStar.Ships;
using GreenStar.Traits;

using Microsoft.AspNetCore.Components;

namespace GreenStar.BlazorUI.Components;

public partial class ActorLineItem
{
    [Inject]
    protected ICommandService CommandService { get; set; } = default!;
    [Parameter]
    public Guid GameId { get; set; }
    [Parameter]
    public Guid PlayerId { get; set; }
    [Parameter]
    public Guid ActorId { get; set; }

    public Actor? Actor { get; set; } = null;

    public IEnumerable<Command> Commands
        => CommandService.GetAllCommands(GameId, PlayerId, ActorId);

    public string PrimaryLine
        => !string.IsNullOrEmpty(Name)
            ? Name + " (" + Actor?.GetType().Name + ")"
            : Actor?.GetType().Name ?? string.Empty;

    public string SecondaryLine
        => Actor switch
        {
            VectorShip vectorShip => vectorShip.Trait<Capable>().ToAbbreviatedString() + " @ Fuel: " + vectorShip.Trait<VectorFlightCapable>().Fuel,
            _ => Actor?.GetType().Name ?? string.Empty,
        };

    public string Name
        => (Actor?.TryGetTrait<Nameable>(out var nameable) ?? false) ? nameable.Name : string.Empty;
    public Populatable? Populatable
        => Actor?.Trait<Populatable>();
    public Associatable? Associatable
        => Actor?.Trait<Associatable>();
    public Hospitality? Hospitality
        => Actor?.Trait<Hospitality>();

    protected override void OnParametersSet()
    {
        if (ActorId != Guid.Empty)
        {
            if (GameHolder.Games.TryGetValue(GameId, out var game))
            {
                Actor = game.Actors.GetActor(ActorId);
            }
        }
    }
}