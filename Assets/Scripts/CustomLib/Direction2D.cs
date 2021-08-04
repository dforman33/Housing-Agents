using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Custom
{
    public class Direction2D
    {
        public static Direction2D Left { get; private set; }
        public static Direction2D Right { get; private set; }
        public static Direction2D Forward { get; private set; }
        public static Direction2D Backward { get; private set; }

        public int DX { get; private set; }
        public int DZ { get; private set; }

        static Direction2D()
        {
            Left = new Direction2D(-1,0);
            Right = new Direction2D(1, 0);
            Forward = new Direction2D(0, 1);
            Backward = new Direction2D(0,-1);
        }
        private Direction2D(int dx, int dz)
        {
            DX = dx;
            DZ = dz; 
        }
        public Coordinate2D Apply(Coordinate2D coordinate)
        {
            return new Coordinate2D(coordinate.X + DX, coordinate.Z + DZ);
        }
        public static Direction2D GetFromTo (Coordinate2D from, Coordinate2D to)
        {
            Direction2D rawDirection = new Direction2D(to.X - from.X, to.Z - from.Z);

            if (rawDirection.IsEqual(Left))
                return Left;
            if (rawDirection.IsEqual(Right))
                return Right;
            if (rawDirection.IsEqual(Forward))
                return Forward;
            if (rawDirection.IsEqual(Backward))
                return Backward;
            throw new System.Exception(string.Format("Can't figure out the direction from coordinate {0} to coordinate {1}", from, to));
        }
        public bool IsEqual(Direction2D other)
        {
            return DX == other.DX && DZ == other.DZ;
        }
        public override string ToString()
        {
            if (this == Left)
                return "Left";
            else if (this == Right)
                return "Right";
            else if (this == Forward)
                return "Forward";
            else if (this == Backward)
                return "Backward";
            throw new System.NotSupportedException("Cannot perform the given direction");
        }

    }
}

