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
        AddSomeOccupied(10);
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
                    Vector3 pos = new Vector3(scale * x, scale * y, scale * z);
                    var go = Instantiate(controller.OpenSpaceCell, new Vector3 (scale * x, scale * y, scale * z), Quaternion.identity);
                    cells[x, y, z] = go.GetComponent<Cell3D>();
                    cells[x, y, z].transform.parent = transform;
                    cells[x, y, z].SetUpCell(x, y, z, new Vector3(scale * x, scale * y, scale * z));
                    Destroy(go);
                }
            }
        }
    }

    public void AddSomeOccupied(byte ID)
    {
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (x == 0 || y == 1 || y == 5)
                    {
                        cells[x, y, z].UpdateCell(20);
                        
                        Debug.Log("Updated cell position: " + cells[x, y, z].cellPos);
                    }
                    else
                    {
                        Destroy(cells[x, y, z].cellPrefab);
                    }
                    
                }
            }
        }


    }


}
