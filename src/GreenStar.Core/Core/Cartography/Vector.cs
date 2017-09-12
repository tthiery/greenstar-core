using System;

namespace GreenStar.Core.Cartography
{
    public class Vector
    {
        public long DeltaX { get; }
        public long DeltaY { get; }
        public Vector(long x, long y)
        {
            DeltaX = x;
            DeltaY = y;
        }
        public long Length
            => (long)Math.Sqrt(LengthSquare);


        public double LengthSquare
            => Math.Pow(DeltaX, 2) + Math.Pow(DeltaY, 2);
    }
}
