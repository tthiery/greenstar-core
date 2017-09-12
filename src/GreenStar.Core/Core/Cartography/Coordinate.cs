using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace GreenStar.Core.Cartography
{
    public class Coordinate
    {
        public long X { get; }
        public long Y { get; }

        public Coordinate(long x, long y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture, "({0},{1})", X, Y);

        public override int GetHashCode()
            => (int)(X + Y) % Int32.MaxValue;

        public override bool Equals(object obj)
        {
            Coordinate o = obj as Coordinate;
            if (o == null)
                return false;
            return (o.X == this.X && o.Y == this.Y);
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
    }
}
