using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Custom namespace
using Custom;

public class Plot3D : MonoBehaviour
{
    [Header("Initial state setting out")]
    public int width = 10; //x length
    public int height = 10; //y height
    public int depth = 10; //z length
    public float scale = 1;

    public int minHeight = 3; //y height
    public int maxHeight; //y height
    public int openSpaceThreshold; //y height

    [HideInInspector] private static Vector3 sunReverseDirection = new Vector3(0.1f, 1, 0.1f);
    [HideInInspector] public Cell3D[,,] cells;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public int[,] greenAreaMap;
    [HideInInspector] public int footprintArea;
    [HideInInspector] public CellStateController controller;
    [HideInInspector] public bool allowTesting = false;

    //EVENTS
    public event Action<int> OnOccupyCell;

    public void ShowSunRays()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (greenAreaMap[x, z] == 1)
                {
                    Vector3 rayStart = cells[x, 0, z].cellPos;
                    Vector3 rayEnd = rayStart + 100 * sunReverseDirection;
                    Debug.DrawLine(rayStart, rayEnd, Color.grey, 10f);
                    Debug.Log($"ray sarts at {rayStart} and ray ends at {rayEnd}");
                }
            }
        }      
    }

    public void ResetBoard(int openSpaceThreshold)
    {
        footprintArea = width * depth;

        foreach (var cell in cells)
        {
            try { Destroy(cell.gameObject); }
            catch { }
        }

        SetupBoard();
        AddPlotConstraints(openSpaceThreshold);
    }
    /// <summary>
    /// Method that instantiates the necessary parameters for the plot3D and generates a three-dimensional array of cells to record and assess moves from players.
    /// </summary>
    public void SetupBoard()
    {
        controller = GetComponent<CellStateController>();

        cells = new Cell3D[width, height, depth];

        footprintArea = width * depth;

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pos = new Vector3(scale * x, scale * y, scale * z) + transform.localPosition;
                    var newGo = Instantiate(controller.EmptyCell, pos, Quaternion.identity);
                    newGo.name = "Cell(" + x + "," + y + "," + z + ")";
                    newGo.transform.parent = transform;
                    cells[x, y, z] = newGo.GetComponent<Cell3D>();
                    cells[x, y, z].SetUpCell(x, y, z, this, controller, 0);
                }
            }
        }
    }

    public void CleanBoard()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    {
                        if (cells[x, y, z] != null) cells[x, y, z].UpdateCell(0);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generates the ground floor, skyline and sets the edge cells as open space
    /// </summary>
    /// <returns>Open space cells at the four sides of the array.</returns>
    public void AddPlotConstraints(int openSpaceThreshold)
    {
        CleanBoard();
        maxHeight = height - 2;

        if (allowTesting)
        {
            heightMap = HeightMapGen.GenerateFixedMap(this, minHeight, maxHeight);
            greenAreaMap = OpenGreenAreaGenerator.GenerateFixedMap(this, openSpaceThreshold); 

        }

        if (!allowTesting)
        {
            heightMap = new HeightMapGen(this, minHeight, maxHeight, 2).heightMap;
            int numAttractors = width * depth < 1 ? 1 + 1 : (int)((width * depth) / 100);
            greenAreaMap = new OpenGreenAreaGenerator(this, numAttractors, openSpaceThreshold).greenAreaMap;
        }

        AddEmptySides();
        AddSkyLine();
        AddGround();
        DisplayOpenSpace();
    }

    /// <summary>
    /// Method to add the open space cells around the 4 vertical edges wihin the array of cells.
    /// It should be called before the AddGround() and AddSkyLine() methods.
    /// </summary>
    /// <returns>Open space cells at the four sides of the array.</returns>
    public void AddEmptySides()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || z == 0 || x == width - 1 || z == depth - 1)
                        cells[x, y, z].UpdateCell(253); // 253 = OPEN AIR CELL
                }
            }
        }
    }
    /// <summary>
    /// Method to add the open space cells above the skyline defined by the heightmap.
    /// It should be called after AddEmptySide() and before the AddSkyLine() methods.
    /// </summary>
    /// <returns>Prefabs at ground floor for display purposes.</returns>
    public void AddGround()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0)
                    {
                        cells[x, y, z].UpdateCell(255); // 255 = GROUND
                    }
                }
            }
        }
    }

    /// <summary>
    /// Changes the color of the open space at Ground Floor and genereates open space cells above.
    /// </summary>
    /// <returns></returns>
    public void DisplayOpenSpace()
    {
        int count = 0;
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (greenAreaMap[x, z] == 1 && y == 0)
                    {
                        cells[x, y, z].UpdateCell(254); // 254 = OPEN GROUND CELL
                        count++;
                    }

                    if (greenAreaMap[x, z] == 1 && y > 0)
                    {
                        cells[x, y, z].UpdateCell(253); // 253 = OPEN AIR CELL
                    }

                }
            }
        }
    }


    /// <summary>
    /// Method to add the open space cells above the skyline defined by the heightmap.
    /// It should be called after AddGround() and AddEmptySide() methods.
    /// </summary>
    /// <returns>Open space positions above the skyline generetaed by the heightmap.</returns>
    public void AddSkyLine()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y > heightMap[x,z]) 
                        cells[x, y, z].UpdateCell(253); // 253 = OPEN AIR CELL
                }
            }
        }

    }

    /// <summary>
    /// Places hard coded external player positions to test the player behaviour.
    /// It works best with these parameters: Width = 10 | Depth = 10 | Height = 12.
    /// </summary>
    /// <returns></returns>
    public void AddSomeOccupied()
    {
        for (int y = 1; y < height -1; y++)
        {
            for (int z = 1; z < depth-1; z++)
            {
                for (int x = 1; x < width-1; x++)
                {
                    if(x > 3 && z > 3 && z<7 && x <7 && y <7 && cells[x,y,z].cellType != CellType.OPENAIR) cells[x, y, z].UpdateCell(5);
                    if (x > 5 && z > 6 && z < 9 && x < 9 && y < 9 && cells[x, y, z].cellType != CellType.OPENAIR) cells[x, y, z].UpdateCell(20);

                }
            }
        }
    }

    /// <summary>
    /// Calculates the number of open space occlusions within the aggregation and draws debug lines to represent them.
    /// </summary>
    /// <returns></returns>
    public void CheckOpenSpaceOcclussions()
    {
        int count = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (greenAreaMap[x, z] == 1)
                {
                    if (RayCast(new Coordinate3D(x, 0, z), sunReverseDirection, "CellOccupied"))
                    {
                        count++;
                        Vector3 start = Navigation.GetPosition(new Coordinate3D(x, 0, z), scale,transform.position);
                        Vector3 end = start + sunReverseDirection.normalized * height;
                        Debug.DrawLine(start, end, Color.blue, 3f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calculates the showding from one coordinate towards the ground floor, using the sunrays direction casts a ray and check for hits.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns> An event that sends a float value linked to the possible reward value between -2 if open ground, 0 if empty, or +0.5 if falls on ground</returns>
    public int ShadowingAnotherCell(Coordinate3D coord)
    {
        int shadowValue = 0;
        Ray ray = new Ray(Navigation.GetPosition(coord, scale, transform.position), -1f * sunReverseDirection);
        RaycastHit hit;

        float maxDist = (float)(width + height + depth);

        if (Physics.Raycast(ray, out hit, maxDist))
        {
            //If the raycast hits a predetermined type triggers a shadowing event with a float value in it.
            if (hit.collider.CompareTag("GroundOpen")) { shadowValue = -1; }
            else if (hit.collider.CompareTag("CellOccupied")) { shadowValue = 0; }
            else { shadowValue = 1; }
        }

        return shadowValue;
    }

    public bool RayCast(Vector3 position, Vector3 direction, string tagName)
    {
        bool punish = false;
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        float maxDist = (float)(width + height + depth);

        if (Physics.Raycast(ray, out hit, maxDist))
        {
            if (hit.collider.CompareTag(tagName))
                punish = true;
        }
        return punish;
    }

    public bool RayCast(Coordinate3D coord, Vector3 direction, string tagName)
    {
        return RayCast(Navigation.GetPosition(coord, scale,transform.position), direction, tagName);
    }

    /// <summary>
    /// Access the cell at the coordinate and occupies the cell for the playerID.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <param name="playerID">The player's unique ID.</param>
    public void OccupyCell(Coordinate3D coord, byte playerID)
    {
        cells[coord.X, coord.Y, coord.Z].UpdateCell(playerID);
        OnOccupyCell?.Invoke(playerID);
    }

    /// <summary>
    /// Returns false if empty and true if the cell is occupied by open space or a housing player.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    public bool IsCellOccupied(Coordinate3D coord)
    {
        if (cells[coord.X, coord.Y, coord.Z].cellType == CellType.EMPTY)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Evaluates the horizontal square neighbours to see if the current position will pack them completely, precluding access to open air.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>1 if precludes position to the right, 2 to the left, 3 to the front, 4 to the back and returns 0 if neighbourhood is not packed.</returns>
    public int IsHorizNeighbourhoodPacked(Coordinate3D coord)
    {
        if (coord.X <= 1 || coord.X >= width - 2 || coord.Y <= 1 || coord.Y >= height - 2 || coord.Z <= 1 || coord.Z >= depth - 2)
            return 0;
        if (ReadSqrHorizNeighbors(coord + Navigation.directions3D[(int)DirectionChoice.Right]) == 15) return 1;
        if (ReadSqrHorizNeighbors(coord + Navigation.directions3D[(int)DirectionChoice.Left]) == 15) return 2;
        if (ReadSqrHorizNeighbors(coord + Navigation.directions3D[(int)DirectionChoice.Front]) == 15) return 3;
        if (ReadSqrHorizNeighbors(coord + Navigation.directions3D[(int)DirectionChoice.Back]) == 15) return 4;

        return 0;
    }

    /// <summary>
    /// Returns an array of 3 values corrrespoding to the empty, occupied and open space binary numbers. 
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>[0] returns the empty positions. [1] returns the occupied positions. [2] returns the open space positions.</returns>
    public int [] GetNeighborhoodValue(Coordinate3D coord)
    {
        int[] result = new int [3];

        result[0] = ReadSqrNeighbors(coord, CellType.EMPTY);
        result[1] = ReadSqrNeighbors(coord, CellType.OCCUPIED);
        result[2] = ReadSqrNeighbors(coord, CellType.OPENAIR);

        return result;
    }

    /// <summary>
    /// Returns binary number of neighbors with the specific cell type. 
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <param name="celltype">The type of cell, generally three values are considered: EMPTY, OCCUPIED, OPEN SPACE.</param>
    /// <returns>An integer that is the binary value of the square neighborhood for the type of cell (empty | occupied | open space).</returns>
    private int ReadSqrNeighbors(Coordinate3D coord, CellType celltype)
    {
        int result = 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Front]).cellType == celltype ? 1 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Back]).cellType == celltype ? 2 :0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Left]).cellType == celltype ? 4: 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Right]).cellType == celltype ? 8 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Up]).cellType == celltype ? 16 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Down]).cellType == celltype ? 32: 0;
        return result;
    }

    /// <summary>
    /// Provides a binary number that represents the horizontal neighboring cells occupied. 
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>An integer that is the binary value of the square horizontal neighborhood for the cell. If all are occupied the result must be equal to 15. </returns>
    public int ReadSqrHorizNeighbors(Coordinate3D coord)
    {
        int result = 0;
        try
        {
            result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Front]).cellType == CellType.OCCUPIED ? 1 : 0;
            result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Back]).cellType == CellType.OCCUPIED ? 2 : 0;
            result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Left]).cellType == CellType.OCCUPIED ? 4 : 0;
            result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Right]).cellType == CellType.OCCUPIED ? 8 : 0;
        }
        catch { }
        return result;
    }

    /// <summary>
    /// Returns binary number of neighbors with the same playerID.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <param name="playerID">The player's unique ID.</param>
    /// <returns>An integer that is the binary value of the square neighborhood of cells with the same ID.</returns>
    public int ReadSelfNeighbor(Coordinate3D coord, byte playerID)
    {
        int result = 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Front]).playerID == playerID ? 1 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Back]).playerID == playerID ? 2 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Left]).playerID == playerID ? 4 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Right]).playerID == playerID ? 8 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Up]).playerID == playerID ? 16 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Down]).playerID == playerID ? 32 : 0;
        return result;
    }

    /// <summary></summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>The cell at the specific coordinate.</returns>
    public Cell3D CellInCoord(Coordinate3D coord)
    {
        try { return cells[coord.X, coord.Y, coord.Z]; }
        catch { return null; }
        
    }

    /// <summary>
    /// Evaluates the plot to count the number of cells with the same ID.
    /// </summary>
    /// <param name="playerID">The player's unique ID.</param>
    /// <returns>An integer that is the sum of all values with the same player ID.</returns>
    public int EqualIDsCount(int playerID)
    {
        int count = 0;
        foreach (var cell in cells) { if (cell.playerID == playerID) count++; }
        return count;
    }

    /// <summary>
    /// Evaluates the plot to count the number of cells of a given type.
    /// </summary>
    /// <param name="celltype">The type of cell, generally three values are considered: EMPTY, OCCUPIED, OPEN SPACE.</param>
    /// <returns>An integer that is the sum of all values with the same player ID.</returns>
    public int CellTypeCount(CellType celltype)
    {
        int count = 0;
        foreach (var cell in cells) { if (cell.cellType == celltype) count++; }
        return count;

    }

    /// <summary>
    /// Counts how many occupied cells do not have access to open air in any face.
    /// </summary>
    public int CountPackedOccupiedCells()
    {
        int count = 0;
        foreach (var cell in cells)
        {
            if (cell.cellType == CellType.OCCUPIED)
                if (ReadSqrHorizNeighbors(cell.coordinate) == 15) count++;
        }
        return count;
    }

    /// <summary>
    /// Evaluates the footprint of a given type of cells, This is the area 2D projection.
    /// </summary>
    /// <param name="celltype">The type of cell, generally three values are considered: EMPTY, OCCUPIED, OPEN SPACE.</param>
    /// <returns>An integer that is the sum of all values with the same player ID.</returns>
    public int CellTypeFootprint(CellType celltype)
    {
        int count = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                bool xzOccupied = false;
                for (int y = 0; y < height; y++)
                {
                    if (cells[x,y,z].cellType == celltype) xzOccupied = true;
                }
                count += xzOccupied ? 1 : 0;
            }
        }
        return count;
    }

    public float AverageOccupiedHeight()
    {
        float average = 0;
        int count = 0;

        foreach (var cell in cells)
        {
            if (cell.cellType == CellType.OCCUPIED)
            {
                count++;
                average += cell.coordinate.Y;
            }
        }
        return (float) Math.Round(average / (double)count, 2);
    }
}
