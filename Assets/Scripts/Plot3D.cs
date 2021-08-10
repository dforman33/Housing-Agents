using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Custom namespace
using Custom;

public class Plot3D : MonoBehaviour
{
    [Header("Initial state setting out")]

    public int width = 10; //x length
    public int height = 12; //y height
    public int depth = 10; //z length
    public float scale = 1;

    public int minHeight = 10; //y height
    public int maxHeight = 4; //y height

    [HideInInspector] public Cell3D[,,] cells;
    [HideInInspector] public int [,] heightMap;
    [HideInInspector] public CellStateController controller;

    private void Awake()
    {
        
        controller = GetComponent<CellStateController>();
        Camera.main.transform.position = new Vector3(-6f, 15.5f, -6f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(33, 45, 0));
        SetupBoard();
        heightMap = new HeightMapGen(this, minHeight, maxHeight,2).heightMap;
        //AddSomeOccupied();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            AddEmptySides();
            AddSkyLine();
            AddGround();
        }        
    }

    /// <summary>
    /// Method that instantiates the necessary parameters for the plot3D and generates a three-dimensional array of cells to record and assess moves from players.
    /// </summary>
    public void SetupBoard()
    {
        cells = new Cell3D[width, height, depth];
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pos = new Vector3(scale * x, scale * y, scale * z) + transform.position;
                    var newGo = Instantiate(controller.EmptyCell, pos, Quaternion.identity);
                    newGo.name = "Cell(" +x+","+y+"," + z + ")";
                    newGo.transform.parent = transform;
                    cells[x, y, z] = newGo.GetComponent<Cell3D>();
                    cells[x, y, z].SetUpCell(x, y, z, this, controller,0);
                }
            }
        }
    }

    /// <summary>
    /// Method to add the open space cells around the 4 vertical edges wihin the array of cells.
    /// It should be called before the AddGround() and AddSkyLine() methods.
    /// </summary>
    /// <returns>Open space cells at the four sides of the array.</returns>
    private void AddEmptySides()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || z ==0 || x == width -1 || z == depth -1)
                        cells[x, y, z].UpdateCell(254); //Open Space
                }
            }
        }

    }
    /// <summary>
    /// Method to add the open space cells above the skyline defined by the heightmap.
    /// It should be called after AddEmptySide() and before the AddSkyLine() methods.
    /// </summary>
    /// <returns>Prefabs at ground floor for display purposes.</returns>
    private void AddGround()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0) cells[x, y, z].UpdateCell(255); //Ground
                }
            }
        }
    }
    /// <summary>
    /// Method to add the open space cells above the skyline defined by the heightmap.
    /// It should be called after AddGround() and AddEmptySide() methods.
    /// </summary>
    /// <returns>Open space positions above the skyline generetaed by the heightmap.</returns>
    private void AddSkyLine()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y > heightMap[x,z]) 
                        cells[x, y, z].UpdateCell(254); //Open Space
                }
            }
        }

    }

    /// <returns>Returns hard coded positions to test the class behaviour.</returns>
    private void AddSomeOccupied()
    {
        for (int y = 1; y < height -1; y++)
        {
            for (int z = 1; z < depth-1; z++)
            {
                for (int x = 1; x < width-1; x++)
                {
                    if(y==1 || y ==2 || y ==2) cells[x, y, z].UpdateCell(5);
                    if (z > 5) cells[x, y, z].UpdateCell(20);
                    if (x==1 && z==5) cells[x, y, z].UpdateCell(14);
                }
            }
        }
    }

    /// <summary>
    /// Access the cell at the coordinate and occupies the cell for the playerID.
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <param name="playerID">The player's unique ID.</param>
    public void OccupyCell(Coordinate3D coord, byte playerID)
    {
        cells[coord.X, coord.Y, coord.Z].UpdateCell(playerID);
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
    /// Returns an array of 3 values corrrespoding to the empty, occupied and open space binary numbers. 
    /// </summary>
    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>[0] returns the empty positions. [1] returns the occupied positions. [2] returns the open space positions.</returns>
    public int [] GetNeighborhoodValue(Coordinate3D coord)
    {
        int[] result = new int [3];

        result[0] = ReadSqrNeighbors(coord, CellType.EMPTY);
        result[1] = ReadSqrNeighbors(coord, CellType.OCCUPIED);
        result[2] = ReadSqrNeighbors(coord, CellType.OPENSPACE);

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
    private int ReadSqrHorizNeighbors(Coordinate3D coord)
    {
        int result = 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Front]).cellType == CellType.OCCUPIED ? 1 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Back]).cellType == CellType.OCCUPIED ? 2 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Left]).cellType == CellType.OCCUPIED ? 4 : 0;
        result += CellInCoord(coord + Navigation.directions3D[(int)DirectionChoice.Right]).cellType == CellType.OCCUPIED ? 8 : 0;
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


    /// <param name="coord">A coordinate3D with the array's position defining the neighborhood.</param>
    /// <returns>The cell at the specific coordinate.</returns>
    public Cell3D CellInCoord(Coordinate3D coord)
    {
        return cells[coord.X, coord.Y, coord.Z];
    }

}
