using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Custom;

public class Plot3D : MonoBehaviour
{
    [Header("Initial state setting out")]

    public int width = 10; //x length
    public int height = 10; //y height
    public int depth = 10; //z length
    public float scale = 1;
    public Vector3 plotOrigin = new Vector3(0, 0, 0);

    [HideInInspector] public Cell3D[,,] cells;
    [HideInInspector] public CellStateController controller;

    private void Start()
    {
        controller = GetComponent<CellStateController>();
        Camera.main.transform.position = new Vector3(-6f, 15.5f, -6f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(33, 45, 0));

        SetupBoard();
        
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
                    var newGo = Instantiate(controller.EmptyCell, new Vector3 (scale * x, scale * y, scale * z), Quaternion.identity);
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
                    if (y == 5 && x ==5)
                    {
                        cells[x, y, z].UpdateCell(20);
                        //Debug.Log("Updated cell position: " + cells[x, y, z].cellPos + " to ID: " + cells[x, y, z].playerID);
                    }
                    if (y == 1 && x == 1)
                    {
                        cells[x, y, z].UpdateCell(30);
                        //Debug.Log("Updated cell position: " + cells[x, y, z].cellPos + " to ID: " + cells[x, y, z].playerID);
                    }
                    if (x == 9 && z == 1)
                    {
                        cells[x, y, z].UpdateCell(40);
                        //Debug.Log("Updated cell position: " + cells[x, y, z].cellPos + " to ID: " + cells[x, y, z].playerID);
                    }
                }
            }
        }


    }


}
