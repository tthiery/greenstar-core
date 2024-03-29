using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;

using Spectre.Console;

namespace GreenStar.Cli;

public record Pick(string Title, Guid ActorId, CommandArgumentDataType DataType);

public static class PickerTerminal
{
    public static void PickCommand(Guid gameId, Guid playerId, List<Pick> picks)
    {
        var facade = new ListingFacade();
        var command = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What [green]list[/] to display?")
                .AddChoices<string>(new[] { "reachable planets", "my planets", "ships" }));


        AnsiConsole.WriteLine($"Search: {command}");

        if (command == "my planets")
        {
            var list = facade.GetAllOwnedPlanets(gameId, playerId)
                .Select(p => new Pick($"{p.StellarType} {p.Name} {p.X}-{p.Y} id:{p.ActorId}", p.ActorId, CommandArgumentDataType.LocatableAndHospitableReference));

            var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Pick>()
                    .Title("What [green]planet[/] to select!")
                    .MoreChoicesText("[gray]more choices[/]")
                    .UseConverter(p => p.Title)
                    .AddChoices(list));

            picks.AddRange(selection);
        }
        if (command == "reachable planets")
        {
            var list = facade.GetAllPlanets(gameId, playerId)
                .Select(p => new Pick($"{p.StellarType} {p.Name} {p.X}-{p.Y} id:{p.ActorId}", p.ActorId, CommandArgumentDataType.LocatableAndHospitableReference));

            var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Pick>()
                    .Title("What [green]planet[/] to select!")
                    .MoreChoicesText("[gray]more choices[/]")
                    .UseConverter(p => p.Title)
                    .AddChoices(list));

            picks.AddRange(selection);
        }


        if (command == "ships")
        {
            var list = facade.GetAllOwnedShips(gameId, playerId)
                .Select(p => new Pick($"{p.ShipType} @ {p.LocationName} {p.X}-{p.Y} id:{p.ActorId}", p.ActorId, CommandArgumentDataType.ActorReference));

            var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Pick>()
                    .Title("What [green]ship[/] to select!")
                    .MoreChoicesText("[gray]more choices[/]")
                    .UseConverter(p => p.Title)
                    .AddChoices(list));

            picks.AddRange(selection);
        }
    }
}

public record PlanetResult(Guid ActorId, string StellarType, string Name, long X, long Y);
public record ShipRecord(Guid ActorId, string ShipType, string? LocationName, long X, long Y);

public class ListingFacade
{
    public IEnumerable<PlanetResult> GetAllPlanets(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var planets = turnManager.Actors.GetActors<Planet, Associatable>();

        return planets.Select(p => new PlanetResult(p.Id, p.GetType().Name, p.Trait<Nameable>().Name, p.Trait<Locatable>().Position.X, p.Trait<Locatable>().Position.Y));
    }
    public IEnumerable<PlanetResult> GetAllOwnedPlanets(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var planets = turnManager.Actors.GetActors<Planet, Associatable>(ass => ass.PlayerId == playerId);

        return planets.Select(p => new PlanetResult(p.Id, p.GetType().Name, p.Trait<Nameable>().Name, p.Trait<Locatable>().Position.X, p.Trait<Locatable>().Position.Y));
    }

    public IEnumerable<ShipRecord> GetAllOwnedShips(Guid gameId, Guid playerId)
    {
        var turnManager = GameHolder.Games[gameId];

        var ships = turnManager.Actors.GetActors<Ship, Associatable>(ass => ass.PlayerId == playerId).ToArray();

        return ships.Select(p =>
        {
            var locationName = (p.Trait<Locatable>(), p.Trait<VectorFlightCapable>()) switch
            {
                ({ HasOwnPosition: true }, { ActiveFlight: true } and var vc) => $"In Flight to {turnManager.Actors.GetActor(vc.TargetActorId)?.Trait<Nameable>().Name}",
                ({ HasOwnPosition: false } and var l, _) => turnManager.Actors.GetActor(l.HostLocationActorId)?.Trait<Nameable>()?.Name,
            };

            return new ShipRecord(p.Id, p.GetType().Name, locationName, p.Trait<Locatable>().GetPosition(turnManager.Actors as IActorContext).X, p.Trait<Locatable>().GetPosition(turnManager.Actors as IActorContext).Y);
        });
    }
}
