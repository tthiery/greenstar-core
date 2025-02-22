using GreenStar.Cartography;

using SkiaSharp;

namespace GreenStar.Renderer;

public record ViewPort<T>(T Width, T Height, T OffsetX, T OffsetY, T Scale);

public class MapControl
{
    public ViewPort<float> ViewPort { get; private set; } = new ViewPort<float>(10, 10, 0, 0, 1.0f);

    public void SetViewPortSize(float viewPortWidth, float viewPortHeight)
    {
        ViewPort = ViewPort with
        {
            Width = viewPortWidth,
            Height = viewPortHeight
        };
    }
    public void SetOffsetToLogicalCoordinate(float logicalOffsetX, float logicalOffsetY)
    {
        ViewPort = ViewPort with
        {
            OffsetX = logicalOffsetX,
            OffsetY = logicalOffsetY
        };
    }
    public void SetScale(float scale)
    {
        ViewPort = ViewPort with
        {
            Scale = scale
        };
    }

    public void MoveViewPortByLogicalVector(float logicalDeltaX, float logicalDeltaY)
    {
        ViewPort = ViewPort with
        {
            OffsetX = ViewPort.OffsetX + logicalDeltaX,
            OffsetY = ViewPort.OffsetY + logicalDeltaY,
        };
    }

    public void ZoomIn(float viewPortPointX = -1, float viewPortPointY = -1, float zoomFactor = 1.25f)
    {
        if (viewPortPointX == -1)
        {
            viewPortPointX = ViewPort.Width / 2;
            viewPortPointY = ViewPort.Height / 2;
        }

        var logicalAnchorCoordinateBefore = MapPointToCoordinate(new SKPoint(viewPortPointX, viewPortPointY));

        ViewPort = ViewPort with
        {
            Scale = ViewPort.Scale * zoomFactor,
        };

        var logicalAnchorCoordinateAfter = MapPointToCoordinate(new SKPoint(viewPortPointX, viewPortPointY)); // Frame got adjusted meanwhile
        var delta = logicalAnchorCoordinateAfter - logicalAnchorCoordinateBefore;

        MoveViewPortByLogicalVector(delta);
    }

    public void ZoomOut(float viewPortPointX = -1, float viewPortPointY = -1, float zoomFactor = 1.25f)
    {
        if (viewPortPointX == -1)
        {
            viewPortPointX = ViewPort.Width / 2;
            viewPortPointY = ViewPort.Height / 2;
        }

        var logicalAnchorCoordinateBefore = MapPointToCoordinate(new SKPoint(viewPortPointX, viewPortPointY));

        ViewPort = ViewPort with
        {
            Scale = ViewPort.Scale / zoomFactor,
        };

        var logicalAnchorCoordinateAfter = MapPointToCoordinate(new SKPoint(viewPortPointX, viewPortPointY)); // Frame got adjusted meanwhile
        var delta = logicalAnchorCoordinateAfter - logicalAnchorCoordinateBefore;

        MoveViewPortByLogicalVector(delta);
    }
    public (float X, float Y) MapLogicalToViewPort(float logicalPointX, float logicalPointY)
    {
        return (
            ViewPort.Width / 2 + (logicalPointX + ViewPort.OffsetX) * ViewPort.Scale,
            ViewPort.Height / 2 + (logicalPointY + ViewPort.OffsetY) * ViewPort.Scale
        );
    }
    public (float X, float Y) MapViewPortToLogical(float viewPortPointX, float viewPortPointY)
    {
        return (
            (int)((viewPortPointX - ViewPort.Width / 2) / ViewPort.Scale - ViewPort.OffsetX),
            (int)((viewPortPointY - ViewPort.Height / 2) / ViewPort.Scale - ViewPort.OffsetY)
        );
    }





    public void MoveViewPortByLogicalVector(Vector coordinateVector)
        => MoveViewPortByLogicalVector(coordinateVector.DeltaX, coordinateVector.DeltaY);

    public SKPoint MapCoordinateToPoint(Coordinate coordinate)
    {
        var (x, y) = MapLogicalToViewPort(coordinate.X, coordinate.Y);

        return new SKPoint(x, y);
    }

    public Coordinate MapPointToCoordinate(SKPoint point)
    {
        var (x, y) = MapViewPortToLogical(point.X, point.Y);

        return new Coordinate((long)x, (long)y);
    }

    public void DrawDebugAtPoint(SKCanvas canvas, SKPoint point)
    {
        using var debugTextPaint = new SKPaint
        {
            Color = SKColors.Pink,
        };
        using var typeface = SKTypeface.FromFamilyName("Arial");
        using var font = typeface.ToFont();

        var coordinate = MapPointToCoordinate(point);

        canvas.DrawLine(point.X, point.Y, point.X + 2, point.Y, debugTextPaint);
        canvas.DrawLine(point.X, point.Y, point.X, point.Y + 2, debugTextPaint);
        canvas.DrawText($"Point: ({point.X}, {point.Y})", new SKPoint(point.X, point.Y + 10), SKTextAlign.Left, font, debugTextPaint);
        canvas.DrawText($"Canvas: {ViewPort.Width} x {ViewPort.Height} @ {ViewPort.Scale} -> {ViewPort.OffsetX} x {ViewPort.OffsetY}", new SKPoint(point.X, point.Y + 20), SKTextAlign.Left, font, debugTextPaint);
        canvas.DrawText($"Coordinate: ({coordinate.X}, {coordinate.Y})", new SKPoint(point.X, point.Y + 30), SKTextAlign.Left, font, debugTextPaint);
    }
}
