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

        /// <summary>
        /// Ensures the coordinate is clamped within certain values.
        /// </summary>
        /// <param name="minX">Minimum x value.</param>
        /// <param name="maxX">Maximum x value.</param>
        /// <param name="minY">Minimum y value.</param>
        /// <param name="maxY">Maximum y value.</param>
        /// <param name="minZ">Minimum z value.</param>
        /// <param name="maxZ">Maximum z value.</param>
        /// <returns>The out parameter IsClamped returns true if clamp is triggered.</returns>

        public void ClampCoordinate(int minX, int maxX, int minY, int maxY, int minZ, int maxZ, out bool IsClamped)
        {
            bool isClamped = false;

            if (X < minX) { X = minX; isClamped = true; }
            if (X > maxX) {X = maxX; isClamped = true; }

            if (Y < minY) {Y = minY; isClamped = true; }
            if (Y > maxY) {Y = maxY; isClamped = true; }

            if (Z < minZ) {Z = minZ; isClamped = true; }
            if (Z > maxZ) {Z = maxZ; isClamped = true; }

            IsClamped = isClamped;
        }

        /// <summary>
        /// Ensures the coordinate is clamped within certain values.
        /// </summary>
        /// <param name="minX">Minimum x value.</param>
        /// <param name="maxX">Maximum x value.</param>
        /// <param name="minY">Minimum y value.</param>
        /// <param name="maxY">Maximum y value.</param>
        /// <param name="minZ">Minimum z value.</param>
        /// <param name="maxZ">Maximum z value.</param>
        /// <returns></returns>

        public void ClampCoordinate(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
        {

            if (X < minX) { X = minX; }
            if (X > maxX) { X = maxX; }

            if (Y < minY) { Y = minY; }
            if (Y > maxY) { Y = maxY; }

            if (Z < minZ) { Z = minZ; }
            if (Z > maxZ) { Z = maxZ; }

        }

        /// <summary>
        /// Static method that ensures the coordinate is clamped within certain values.
        /// </summary>
        /// <param name="coordinate">The coordinate to clamp.</param>
        /// <param name="minX">Minimum x value.</param>
        /// <param name="maxX">Maximum x value.</param>
        /// <param name="minY">Minimum y value.</param>
        /// <param name="maxY">Maximum y value.</param>
        /// <param name="minZ">Minimum z value.</param>
        /// <param name="maxZ">Maximum z value.</param>
        /// <returns>A clamped coordinate.</returns>
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
