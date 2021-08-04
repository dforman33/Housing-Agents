
namespace Custom
{
    public class Coordinate2D
    {
        public Coordinate2D(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X { get; private set; }
        public int Z { get; private set; }
        public static Coordinate2D operator -(Coordinate2D coordinate1, Coordinate2D coordinate2)
        {
            return new Coordinate2D(coordinate1.X - coordinate1.X, coordinate2.Z - coordinate2.Z);
        }

        public static Coordinate2D operator +(Coordinate2D coordinate1, Coordinate2D coordinate2)
        {
            return new Coordinate2D(coordinate1.X + coordinate1.X, coordinate2.Z + coordinate2.Z);
        }

        public Coordinate2D Clone()
        {
            return new Coordinate2D(X, Z);
        }

        public bool Equals(Coordinate2D other)
        {
            return X == other.X && Z == other.Z;
        }
        public bool IsInRange(int minX, int maxX, int minZ, int maxZ)
        {
            return X >= minX && X <= maxX && Z >= minZ && Z <= maxZ;
        }

        public void Set(Coordinate2D coordinate)
        {
            X = coordinate.X;
            Z = coordinate.Z;
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Z:{1}", X, Z);
        }
    }
}


