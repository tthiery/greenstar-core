using GreenStar.AppService.Actor;
using GreenStar.Traits;

using Spectre.Console;

namespace GreenStar.Cli;

public record Pick(string Title, Guid ActorId, CommandArgumentDataType DataType);

public static class PickerTerminal
{
    public static void PickCommand(Guid gameId, Guid playerId, List<Pick> picks)
    {
        ISearchService facade = new SearchDomainService();
        var command = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What [green]list[/] to display?")
                .AddChoices<string>(new[] { "reachable planets", "my planets", "ships" }));


        AnsiConsole.WriteLine($"Search: {command}");

        if (command == "my planets")
        {
            var list = facade.GetAllAssociatedPlanets(gameId, playerId)
                .Select(p => new Pick($"Planet {p.Trait<Nameable>().Name} {p.Trait<Locatable>().Position.X}-{p.Trait<Locatable>().Position.Y} id:{p.Id}", p.Id, CommandArgumentDataType.LocatableAndHospitableReference));

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
            var list = facade.GetAllKnownPlanets(gameId, playerId)
                .Select(p => new Pick($"Planet {p.Trait<Nameable>().Name} {p.Trait<Locatable>().Position.X}-{p.Trait<Locatable>().Position.Y} id:{p.Id}", p.Id, CommandArgumentDataType.LocatableAndHospitableReference));

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
