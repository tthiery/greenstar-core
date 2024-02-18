using GreenStar.Cartography;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
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

        var actorContext = turnEngine?.Actors;

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


        var fontCollection = new FontCollection();
        var family = fontCollection.Add("Roboto-Regular.ttf");
        var font = family.CreateFont(12, FontStyle.Regular);

        using (var image = new Image<Rgba32>((int)imageWidth, (int)imageHeight))
        {
            image.Mutate(ctx =>
            {
                foreach (var actor in allActors)
                {
                    if (actor.TryGetTrait<Locatable>(out var locatable))
                    {
                        var point = MapCoordinate(locatable.Position);

                        if (actor is VectorShip && actor.TryGetTrait<VectorFlightCapable>(out var vectorFlight) && vectorFlight.ActiveFlight)
                        {
                            var targetPoint = MapCoordinate(locatable.Position + vectorFlight.RelativeMovement);
                            ctx.DrawLine(SixLabors.ImageSharp.Color.White, 2, point, targetPoint);
                        }

                        if (actor is ExactLocation)
                        {
                            var ellipse = new EllipsePolygon(point, 2);

                            ctx.Fill(SixLabors.ImageSharp.Color.Red, ellipse);

                            ctx.DrawText(new RichTextOptions(font)
                            {
                                Origin = new PointF(point.X, point.Y + 10),
                                HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Center
                            }, actor.Trait<Nameable>().Name, SixLabors.ImageSharp.Color.White);
                        }

                        if (actor is Sun or Planet)
                        {
                            var color = actor switch
                            {
                                Sun => SixLabors.ImageSharp.Color.Yellow,
                                Planet => SixLabors.ImageSharp.Color.Blue,
                                _ => SixLabors.ImageSharp.Color.White
                            };

                            ctx.Fill(color, new EllipsePolygon(point, 5));
                            ctx.DrawText(new RichTextOptions(font)
                            {
                                Origin = new PointF(point.X, point.Y + 10),
                                HorizontalAlignment = SixLabors.Fonts.HorizontalAlignment.Center
                            }, actor.Trait<Nameable>().Name, SixLabors.ImageSharp.Color.White);

                            if (actor.TryGetTrait<Associatable>(out var associatable))
                            {
                                if (associatable.PlayerId != Guid.Empty)
                                {
                                    var ellipseOwner = new EllipsePolygon(point, 7);

                                    ctx.Draw(SixLabors.ImageSharp.Color.White, 1, ellipseOwner);
                                }
                            }

                            if (actor.TryGetTrait<Hospitality>(out var hospitality))
                            {
                                if (hospitality.ActorIds.Count > 0)
                                {
                                    var p2 = new PointF(point.X + 7, point.Y - 7);
                                    var ellipseShips = new EllipsePolygon(p2, 2);

                                    ctx.Fill(SixLabors.ImageSharp.Color.Red, ellipseShips);
                                }
                            }
                        }
                    }
                }
            });

            image.Save("universe.png");
        }

        PointF MapCoordinate(Coordinate coordinate)
        {
            return new PointF(
                imageOffset + (coordinate.X + offsetX) * scale,
                imageOffset + (coordinate.Y + offsetY) * scale
            );
        }
    }

}