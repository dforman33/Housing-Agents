using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public static class Navigation
    {
        public static Coordinate2D[] directions2D = new Coordinate2D[]
        {
        new Coordinate2D(1,0),
        new Coordinate2D(0,1),
        new Coordinate2D(-1,0),
        new Coordinate2D(0,-1)
        };

        public static Coordinate3D[] directions3D = new Coordinate3D[]
        {
            new Coordinate3D(1,0,0),
            new Coordinate3D(-1,0,0),
            new Coordinate3D(0,1,0),
            new Coordinate3D(0,-1,0),
            new Coordinate3D(0,0,1),
            new Coordinate3D(0,0,-1)
        };


        /* NAVIGATION TOOLS
      * This is a set of tools helping navigate the plot
      * 
      * 
      */

        public static Vector3 GetPosition(int x, int z, float cellSize, Vector3 originPos)
        {
            return new Vector3(x, 0, z) * cellSize + originPos;
        }

        public static Coordinate2D GetCoordinates(Vector3 worldPos, float cellSize, Vector3 originPos)
        {
            int x = Mathf.RoundToInt((worldPos.x - originPos.x) / cellSize);
            int z = Mathf.RoundToInt((worldPos.z - originPos.z) / cellSize);
            return new Coordinate2D(x, z);
        }
        public static bool IsWithinPlot(Vector3 position, float cellSize, Vector3 originPos, int xMax, int zMax)
        {
            if (position.x > originPos.x && position.x < (originPos.x + xMax * cellSize) && position.z > originPos.z && position.z < (originPos.z + zMax * cellSize))
                return true;
            else
                return false;
        }

        public static Coordinate2D CheckCollisionOnPlot(Vector3 originPos, float cellSize, int xMax, int zMax,
            out bool remove, out bool isWithinPlot)
        {
            remove = false;
            isWithinPlot = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.normal.x == 0 || hit.normal.y == 0 || hit.normal.z == 0)
                {
                    Vector3 pos = hit.point;
                    if (IsWithinPlot(pos, cellSize, originPos, xMax, zMax))
                    {
                        pos = hit.point + hit.normal * 0.5f * cellSize;
                        isWithinPlot = true;
                        return GetCoordinates(pos, cellSize, originPos);
                    }
                }
            }

            else if (Physics.Raycast(ray, out hit, 100.0f) && Input.GetMouseButtonDown(1))
            {
                if (hit.collider.gameObject != null)
                {
                    Vector3 pos = hit.point;
                    pos = new Vector3(pos.x, 0, pos.z);
                    if (IsWithinPlot(pos, cellSize, originPos, xMax, zMax))
                    {
                        remove = true;
                        isWithinPlot = true;
                        return GetCoordinates(pos, cellSize, originPos);
                    }
                }
            }
            return new Coordinate2D(-1, -1);
        }
    }

}