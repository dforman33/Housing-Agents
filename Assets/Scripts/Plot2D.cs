using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

public class Plot2D
{
    private int width; //x length
    private int depth; //z length
    private Vector3 plotOrigin; //bottom left corner
    private byte maxHeight; //this is the absolute maximum height for the aggregation =
     
    //These are the key parameters
    public Cell2D[,] cells;
    private byte [,] heightMap;
    private byte [,] openSpaceMap;

    public Plot2D(int width, int depth, Vector3 origin, byte maxHeight)
    {
        this.width = width;
        this.depth = depth;
        plotOrigin = origin;
        this.maxHeight = maxHeight;

        cells = new Cell2D[width, depth];
        heightMap = new byte [width, depth];
        openSpaceMap = new byte [width, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                cells[x, z] = new Cell2D(x, z);
                heightMap[x, z] = maxHeight; //if 2D then max height 1
                openSpaceMap [x,z] = 0; // 0 = developable space, 1 = open space
            }
        }
    }

    public Plot2D(int width, int depth, Vector3 origin) : this(width, depth, origin, 1) { }

    public void UpdatePlot2D(Coordinate2D coord, byte newState_ID) 
    {
        UpdateCell(coord, newState_ID);
    }

    private void UpdateCell(Coordinate2D coord, byte newState)
    {
        cells[coord.X, coord.Z].updateCell(newState);
    }

}
