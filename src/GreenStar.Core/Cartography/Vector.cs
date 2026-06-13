using System;
using System.Globalization;

namespace GreenStar.Cartography;

public class Vector
{
    public long DeltaX { get; }
    public long DeltaY { get; }
    public Vector(long x, long y)
    {
        DeltaX = x;
        DeltaY = y;
    }

    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{0},{1}", DeltaX, DeltaY);

    public static implicit operator Vector(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("value not set", nameof(value));
        }

        var coordinates = value.Split(',');

        if (coordinates.Length != 2)
        {
            throw new ArgumentException("value is not a coordinate string");
        }

        if (!(long.TryParse(coordinates[0], out long x) && long.TryParse(coordinates[1], out long y)))
        {
            throw new ArgumentException("x or y is not a int");
        }

        return new Vector(x, y);
    }

    public long Length
        => (long)Math.Sqrt(LengthSquare);


    public double LengthSquare
        => Math.Pow(DeltaX, 2) + Math.Pow(DeltaY, 2);
}
