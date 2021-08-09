using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public class Coordinate3D
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Coordinate3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(Coordinate3D other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }
        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}, Z:{2}", X, Y, Z);
        }
        public bool IsInRange(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            return X >= minX && X <= maxX && Y >= minY && Y <= maxY && Z >= minZ && Z <= maxZ;
        }
        public Coordinate3D Clone()
        {
            return new Coordinate3D(X, Y, Z);
        }
        public void Set(Coordinate3D coordinate)
        {
            X = coordinate.X;
            Y = coordinate.Y;
            Z = coordinate.Z;
        }
        public static Coordinate3D operator +(Coordinate3D coordinate1, Coordinate3D coordinate2)
        {
            return new Coordinate3D(coordinate1.X + coordinate2.X, coordinate1.Y + coordinate2.Y, coordinate1.Z + coordinate2.Z);
        }
        public static Coordinate3D operator -(Coordinate3D coordinate1, Coordinate3D coordinate2)
        {
            return new Coordinate3D(coordinate1.X - coordinate2.X, coordinate1.Y - coordinate2.Y,coordinate1.Z - coordinate2.Z);
        }

        public void ClampCoordinate(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            if (X < minX) X = minX;
            if (X > maxX) X = maxX;

            if (Y < minY) Y = minY;
            if (Y > maxY) Y = maxY;

            if (Z < minZ) Z = minZ;
            if (Z > maxZ) Z = maxZ;
        }


        public static Coordinate3D ClampCoordinate(Coordinate3D coordinate, int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {
            if (coordinate.X < minX) coordinate.X = minX;
            if (coordinate.X > maxX) coordinate.X = maxX;

            if (coordinate.Y < minY) coordinate.Y = minY;
            if (coordinate.Y > maxY) coordinate.Y = maxY;

            if (coordinate.X < minZ) coordinate.Z = minZ;
            if (coordinate.X > maxZ) coordinate.Z = maxZ;

            return coordinate;
        }

    }
}
