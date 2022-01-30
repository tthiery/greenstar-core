

// TODO: against a facade (to allow later switch to a service layer)
// TODO: persistence
// TODO: game setup
// TODO: do turn
// TODO: see events
// TODO: see research tree
// TODO: execute commands
// ...


using GreenStar.Core;
using GreenStar.Core.Cartography;
using GreenStar.Core.Traits;
using GreenStar.Stellar;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;

using Spectre.Console;
using GreenStar.Core.TurnEngine;
using GreenStar.Algorithms;
using System.Reflection;
using GreenStar.Core.TurnEngine.Players;

TurnManager turnManager = null;

while (true)
{
    var command = AnsiConsole.Prompt(
        new TextPrompt<string>("What's [green]next[/]?")
            .InvalidChoiceMessage("[red]That's not a valid command[/]")
            .DefaultValue("turn")
            .AddChoice("setup")
            .AddChoice("map")
            .AddChoice("turn")
            .AddChoice("exit"));


    var rule = new Rule($"[red]{command}[/]");
    rule.Alignment = Justify.Left;
    AnsiConsole.Write(rule);

    if (command == "setup")
    {
        turnManager = Setup();
    }

    if (command == "map")
    {
        Map(turnManager.Game);
    }

    if (command == "turn")
    {
        if (turnManager is not null)
        {
            foreach (var player in turnManager.Game.Players)
            {
                turnManager.FinishTurn(player.Id);
            }

            AnsiConsole.WriteLine($"Turn {turnManager.Game.Turn} completed");
        }
    }

    if (command == "exit")
    {
        break;
    }
}

TurnManager Setup()
{
    var setupFacade = new SetupFacade();
    var gameTypes = setupFacade.GetGameTypes();
    var stellarTypes = setupFacade.GetStellarTypes();

    var selectedType = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Which Game [green]Type[/] you want to start?")
        .PageSize(4)
        .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
        .AddChoices(gameTypes.Select(g => g.Name)));

    AnsiConsole.WriteLine($"Game Type: {selectedType}");

    var nrOfSystemPlayers = AnsiConsole.Ask<int>("How many System Players?");

    var selectedStellarType = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Define the [green]stellar type[/] you want to play?")
        .PageSize(4)
        .MoreChoicesText("[grey](Move up and down to reveal more types)[/]")
        .AddChoices(stellarTypes.Select(g => g.Name)));
    AnsiConsole.WriteLine($"Stellar Type: {selectedStellarType}");

    var stellarType = stellarTypes.First(t => t.Name == selectedStellarType);

    var stellarArgs = new int[stellarType.Properties.Length];

    for (int i = 0; i < stellarType.Properties.Length; i++)
    {
        stellarArgs[i] = AnsiConsole.Ask<int>($"[green]{stellarType.Properties[i].Name}[/]");

    }

    var builder = new TurnManagerBuilder()
        .AddCoreTranscript()
        .AddStellarTranscript()
        .AddPlayer(new HumanPlayer(Guid.NewGuid(), "Red", Array.Empty<Guid>(), 22, 1));

    for (int idx = 0; idx < nrOfSystemPlayers; idx++)
    {
        builder.AddPlayer(new SystemPlayer());
    }
    var turnEngine = builder.Build();

    var t = Type.GetType("GreenStar.Algorithms.GeneratorAlgorithms, GreenStar.Stellar") ?? throw new InvalidOperationException("failed to find stellarstrategies");

    var method = t.GetMethod(selectedStellarType);
    var args = new object[2 + stellarArgs.Length];
    args[0] = (IActorContext)turnEngine.Game;
    args[1] = GeneratorMode.PlanetOnly;
    Array.Copy(stellarArgs, 0, args, 2, stellarArgs.Length);
    method.Invoke(null, args);

    return turnEngine;
}


void Map(IActorContext actorContext)
{
    if (actorContext is null)
    {
        AnsiConsole.WriteLine("[red]No game loaded[/]");

        return;
    }

    long minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
    var allActors = actorContext.AsQueryable();

    // find used coordinates
    foreach (var actor in allActors)
    {
        if (actor.TryGetTrait<Locatable>(out var locatable))
        {
            if (locatable.HasOwnPosition)
            {
                var (x, y) = locatable.Position;

                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }
        }
    }

    // create width and height
    var universeWidth = maxX - minX;
    var universeHeight = maxY - minY;
    var offsetX = -1 * minX;
    var offsetY = -1 * minY;

    var maxValue = universeWidth > universeHeight ? universeWidth : universeHeight;
    var scale = (maxValue > 1000) ? 1000.0f / maxValue : 1.0f;

    var imageOffset = maxValue * scale * 0.1f;
    var imageWidth = universeWidth * scale * 1.2f;
    var imageHeight = universeHeight * scale * 1.2f;

    using (var image = new Image<Rgba32>((int)imageWidth, (int)imageHeight))
    {
        foreach (var actor in allActors)
        {
            if (actor.TryGetTrait<Locatable>(out var locatable))
            {
                var point = new PointF(
                    imageOffset + (locatable.Position.X + offsetX) * scale,
                    imageOffset + (locatable.Position.Y + offsetY) * scale
                );
                var ellipse = new SixLabors.ImageSharp.Drawing.EllipsePolygon(point, 5);

                var color = actor switch
                {
                    Sun => SixLabors.ImageSharp.Color.Yellow,
                    Planet => SixLabors.ImageSharp.Color.Blue,
                    _ => SixLabors.ImageSharp.Color.White
                };

                image.Mutate(ctx => ctx.Fill(color, ellipse));
            }
        }
        image.Save("universe.png");
    }
}

public record GameType(string Name);
public record StellarType(string Name, StellarTypeProperty[] Properties);
public record StellarTypeProperty(string Name);

public class SetupFacade
{
    public IEnumerable<GameType> GetGameTypes()
    {
        return Directory.GetDirectories("../../data").Select(d => new GameType(new DirectoryInfo(d).Name));
    }

    public IEnumerable<StellarType> GetStellarTypes()
    {
        var t = Type.GetType("GreenStar.Algorithms.GeneratorAlgorithms, GreenStar.Stellar") ?? throw new InvalidOperationException("failed to find stellarstrategies");

        var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);

        var result = methods.Select(m => new StellarType(m.Name, m.GetParameters()
            .Where(p => p.Name != "actorContext")
            .Where(p => p.Name != "mode")
            .Select(p => new StellarTypeProperty(p.Name)).ToArray()));
        return result;
    }
}