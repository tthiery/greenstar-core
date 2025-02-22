using GreenStar.AppService;
using GreenStar.Ships;
using GreenStar.Stellar;
using GreenStar.Traits;

using SkiaSharp;

namespace GreenStar.Renderer;

public class SelectedItem
    : Actor
{
    public SelectedItem()
    {
        AddTrait<Locatable>();
    }
}

public class SkiaSharpRenderer
{
    public MapControl MapAlgorithm { get; } = new();

    public void InitViewPort(Guid gameId)
    {
        var turnEngine = GameHolder.Games[gameId];

        var actorContext = turnEngine?.Actors;

        if (actorContext is null)
        {
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

        MapAlgorithm.SetScale(scale);
        MapAlgorithm.SetOffsetToLogicalCoordinate(offsetX, offsetY);
    }

    public void MapGame(Guid gameId, SKCanvas canvas)
    {
        var turnEngine = GameHolder.Games[gameId];

        var actorContext = turnEngine?.Actors;

        if (actorContext is null)
        {
            return;
        }

        var allActors = actorContext.AsQueryable();

        var typeface = SKTypeface.FromFamilyName("Arial");
        var font = typeface.ToFont();

        using var neutralAnnotationPaint = new SKPaint
        {
            Color = SKColors.White,
        };

        using var textPaint = new SKPaint
        {
            Color = SKColors.White,
        };
        using var shipLinePaint = new SKPaint
        {
            Color = SKColors.White,
            StrokeWidth = 2,
        };
        using var exactLocationPaint = new SKPaint
        {
            Color = SKColors.Red,
        };
        using var planetPaint = new SKPaint
        {
            Color = SKColors.Blue,
        };
        using var sunPaint = new SKPaint
        {
            Color = SKColors.Yellow,
        };


        canvas.Clear(SKColor.Parse("#1E2952"));
        MapAlgorithm.DrawDebugAtPoint(canvas, new SKPoint(0, 0));

        foreach (var actor in allActors)
        {
            if (actor.TryGetTrait<Locatable>(out var locatable))
            {
                var point = MapAlgorithm.MapCoordinateToPoint(locatable.GetPosition(actorContext));

                if (actor is VectorShip && actor.TryGetTrait<VectorFlightCapable>(out var vectorFlight) && vectorFlight.ActiveFlight)
                {
                    var targetPoint = MapAlgorithm.MapCoordinateToPoint(locatable.Position + vectorFlight.RelativeMovement);

                    canvas.DrawLine(point, targetPoint, shipLinePaint);
                }

                if (actor is ExactLocation)
                {
                    canvas.DrawCircle(point, 2, exactLocationPaint);
                    canvas.DrawText(actor.Trait<Nameable>().Name, new SKPoint(point.X, point.Y + 10), SKTextAlign.Center, font, textPaint);
                }

                if (actor is Sun or Planet)
                {
                    var color = actor switch
                    {
                        Sun => sunPaint,
                        Planet => planetPaint,
                        _ => shipLinePaint
                    };

                    canvas.DrawCircle(point, 5, color);
                    canvas.DrawText(actor.Trait<Nameable>().Name, new SKPoint(point.X, point.Y + 10), SKTextAlign.Center, font, textPaint);

                    if (actor.TryGetTrait<Associatable>(out var associatable))
                    {
                        if (associatable.PlayerId != Guid.Empty)
                        {
                            canvas.DrawCircle(point, 7, neutralAnnotationPaint);
                        }
                    }

                    if (actor.TryGetTrait<Hospitality>(out var hospitality))
                    {
                        if (hospitality.ActorIds.Count > 0)
                        {
                            var p2 = new SKPoint(point.X + 7, point.Y - 7);
                            canvas.DrawCircle(p2, 2, neutralAnnotationPaint);
                        }
                    }
                }

                if (actor is SelectedItem)
                {
                    var position = locatable.GetPosition(actorContext as IActorContext);
                    var pointSelection = MapAlgorithm.MapCoordinateToPoint(position);

                    canvas.DrawCircle(pointSelection, 20, new SKPaint { Color = SKColors.Yellow, Style = SKPaintStyle.Stroke });
                }
            }
        }
    }
}