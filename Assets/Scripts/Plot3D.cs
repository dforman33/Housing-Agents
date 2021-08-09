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
    public void OccupyCell(Coordinate3D coord, byte playerID)
    {
        cells[coord.X, coord.Y, coord.Z].UpdateCell(playerID);
    }

    /// <summary>
    /// Returns false if empty and true if the cell is occupied by open space or a housing player.
    /// </summary>
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
    /// Returns binary number of neighbors with the same playerID.
    /// </summary>
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

    public Cell3D CellInCoord(Coordinate3D coord)
    {
        return cells[coord.X, coord.Y, coord.Z];
    }

}
