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
    public GameObject basePrefab;

    private CellBehaviour[,,] cellStates;
    [HideInInspector] public CellStateController controller;

    private void Start()
    {
        controller = GetComponent<CellStateController>();
        SetupBoard();
    }

    private void SetupBoard()
    {
        cellStates = new CellBehaviour[width, height, depth];
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector3 pos = new Vector3(scale * x, scale * y, scale * z);

                    var go = Instantiate(controller.EmptyCell, pos, Quaternion.identity);
                    CellBehaviour cell = go.GetComponent<CellBehaviour>();
                    cellStates[x, y, z] = cell;
                    cellStates[x, y, z].SetupCell(x, y, z);
                    cellStates[x, y, z].UpdateCell(State.EMPTY, 0);

                    //cell.UpdateCell(State.EMPTY, 0);
                    //cellStates[x, y, z] = cell;

                    if (y == 0)
                    {
                        var pl = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        pl.transform.position = pos;
                        float plScale = 0.095f;
                        pl.transform.localScale = new Vector3(scale * plScale, scale * plScale, scale * plScale);
                    }
                }

            }

        }
    }







}
