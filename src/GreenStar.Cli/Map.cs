using GreenStar.Core;
using GreenStar.Core.Traits;
using GreenStar.Stellar;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Spectre.Console;

namespace GreenStar.Cli;


public static class Map
{
    public static void MapCommand(Guid gameId)
    {
        var turnEngine = GameHolder.Games[gameId];

        IActorContext? actorContext = turnEngine?.Game;

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

}