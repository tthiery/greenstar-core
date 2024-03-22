using GreenStar.AppService.Setup;

using Spectre.Console;

namespace GreenStar.Cli;

public static class Setup
{
    public static async Task<(Guid, Guid)> SetupCommand()
    {
        var setupFacade = new SetupDomainService();
        var gameTypes = setupFacade.GetGameTypes();
        var stellarTypes = setupFacade.GetStellarTypes();

        var selectedGameType = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Which Game [green]Type[/] you want to start?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(gameTypes.Select(g => g.Name)));

        AnsiConsole.WriteLine($"Game Type: {selectedGameType}");

        var nrOfSystemPlayers = AnsiConsole.Ask<int>("How many AI Players?");

        var selectedStellarType = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Define the [green]stellar type[/] you want to play?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(stellarTypes.Select(g => g.Name)));
        AnsiConsole.WriteLine($"Stellar Type: {selectedStellarType}");

        var stellarType = stellarTypes.First(t => t.Name == selectedStellarType);

        for (int i = 0; i < stellarType.Arguments.Length; i++)
        {
            stellarType.Arguments[i] = stellarType.Arguments[i] with
            {
                Value = AnsiConsole.Ask<double>($"[green]{stellarType.Arguments[i].DisplayName}[/]", stellarType.Arguments[i].Value),
            };
        }

        var game = await setupFacade.CreateGameAsync(selectedGameType, nrOfSystemPlayers, stellarType);

        return (game.Id, game.HumanPlayerId);
    }

    public static async Task<(Guid, Guid)> LoadCommand()
    {
        var setupFacade = new SetupDomainService();
        var list = await setupFacade.GetPersistedGamesAsync();

        var selectedGame = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("Define the [green]game[/] you want to load?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
            .AddChoices(list.Select(g => g.Id.ToString())));
        AnsiConsole.WriteLine($"Game: {selectedGame}");

        var guid = Guid.Parse(selectedGame);

        var game = list.FirstOrDefault(g => g.Id == guid);

        game = await setupFacade.LoadGameAsync(game);

        return (game.Id, game.HumanPlayerId);
    }
}
