using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace GreenStar.Cartography;

public class Coordinate : IEquatable<Coordinate>
{
    public static readonly Coordinate Zero = (0, 0);

    public long X { get; }
    public long Y { get; }

    public Coordinate(long x, long y)
    {
        X = x;
        Y = y;
    }


    public override string ToString()
        => string.Format(CultureInfo.InvariantCulture, "{0},{1}", X, Y);

    public override bool Equals(object? obj)
        => obj is Coordinate && Equals((Coordinate)obj);

    public bool Equals(Coordinate? other)
        => X == other?.X && Y == other?.Y;

    public override int GetHashCode()
        => (int)(X ^ Y);

    public static bool operator ==(Coordinate left, Coordinate right)
        => left.Equals(right);

    public static bool operator !=(Coordinate left, Coordinate right)
        => !left.Equals(right);

    public static implicit operator Coordinate((int x, int y) value)
        => new Coordinate(value.x, value.y);

    public static implicit operator Coordinate((long x, long y) value)
        => new Coordinate(value.x, value.y);

    public static Coordinate ToCoordinate(long x, long y)
        => new Coordinate(x, y);

    public void Deconstruct(out long x, out long y)
    {
        x = X;
        y = Y;
    }
    public static Coordinate operator -(Coordinate neg)
        => new Coordinate(-1 * neg.X, -1 * neg.Y);

    public static Vector operator +(Coordinate p1, Coordinate p2)
        => new Vector(p1.X + p2.X, p1.Y + p2.Y);

    public static Vector operator -(Coordinate p1, Coordinate p2)
        => new Vector(p1.X - p2.X, p1.Y - p2.Y);

    public static Coordinate operator +(Coordinate c, Vector v)
        => new Coordinate(c.X + v.DeltaX, c.Y + v.DeltaY);

    public static Coordinate operator -(Coordinate c, Vector v)
        => new Coordinate(c.X - v.DeltaX, c.Y - v.DeltaY);



    public static implicit operator Coordinate(string value)
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

        return new Coordinate(x, y);
    }
}
