using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject cellInitialPrebab;
    public CellState[,] globalState; 

    [SerializeField] public int width = 10; //x length
    [SerializeField] public int depth = 10; //z length
    [SerializeField] public float scale = 1;
    [SerializeField] public Vector3 plotOrigin = new Vector3(0, 0, 0);

    private float arrayScale;
    private float baseOffset;
    private float voxelFactor = 0.9f;
    private Vector3 voxelScale;

    private void Start()
    {
        SetupBoard();
        Debug.Log("Board set up done");
    }

    private void SetupBoard()
    {
        arrayScale = scale * 2;
        baseOffset = scale;
        voxelScale = new Vector3(voxelFactor, voxelFactor, voxelFactor);

        globalState = new CellState[width, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var go = Instantiate(cellInitialPrebab, new Vector3(x, 0, z), new Quaternion());
                globalState[x, z] = go.GetComponent<CellState>();
            }
        }

    }

}
