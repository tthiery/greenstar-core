// TODO: against a facade (to allow later switch to a service layer)
// TODO: persistence
// TODO: see research tree
// TODO: execute commands
// ...


using Spectre.Console;
using GreenStar.Cli;

var gameId = Guid.Empty;
var playerId = Guid.Empty;
var picks = new List<Pick>();

while (true)
{
    var command = AnsiConsole.Prompt(
        new TextPrompt<string>("What's [green]next[/]?")
            .InvalidChoiceMessage("[red]That's not a valid command[/]")
            .DefaultValue("turn")
            .AddChoices<string>(new[] { "map", "pick", "command", "turn", "log", "setup", "exit" }));


    var rule = new Rule($"[red]{command}[/]");
    rule.Alignment = Justify.Left;
    AnsiConsole.Write(rule);

    if (command == "setup")
    {
        (gameId, playerId) = Setup.SetupCommand();
    }

    if (command == "map")
    {
        Map.MapCommand(gameId);
    }

    if (command == "pick")
    {
        PickerTerminal.PickCommand(gameId, playerId, picks);
    }

    if (command == "command")
    {
        CommandTerminal.CommandCommand(gameId, playerId, picks);
    }

    if (command == "turn")
    {
        picks.Clear();

        Turn.TurnCommand(gameId, playerId);
        Map.MapCommand(gameId);
    }

    if (command == "log")
    {
        Turn.LogCommand(gameId, playerId);
    }

    if (command == "exit")
    {
        break;
    }
}
