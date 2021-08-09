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

    [HideInInspector] public Cell3D[,,] cells;
    [HideInInspector] public CellStateController controller;

    private void Awake()
    {
        controller = GetComponent<CellStateController>();
        Camera.main.transform.position = new Vector3(-6f, 15.5f, -6f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(33, 45, 0));
        SetupBoard();
        AddSomeOccupied();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            AddSomeOccupied();
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

    public void AddSomeOccupied()
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if(y==0 || y ==1 || y ==2) cells[x, y, z].UpdateCell(5);
                    if (z > 5) cells[x, y, z].UpdateCell(20);
                    if (x==1 && z==5) cells[x, y, z].UpdateCell(14);
                }
            }
        }
    }

    public void OccupyCell(Coordinate3D coord, byte playerID)
    {
        cells[coord.X, coord.Y, coord.Z].UpdateCell(playerID);
    }

    public bool IsCellOccupied(Coordinate3D coord)
    {
        if (cells[coord.X, coord.Y, coord.Z].cellType == CellType.EMPTY)
            return false;
        else
            return true;
    }


}
