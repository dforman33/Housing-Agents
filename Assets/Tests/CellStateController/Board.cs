using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Custom;

public class Board : MonoBehaviour
{
    public GameObject cellInitialPrebab;
    public CellState[,] globalState; 
    
    [SerializeField] public int width = 10; //x length
    [SerializeField] public int depth = 10; //z length
    [SerializeField] public float scale = 1;
    [SerializeField] public Vector3 plotOrigin = new Vector3(0, 0, 0); //bottom left corner
    private byte maxHeight; //this is the absolute maximum height for the aggregation

    private float arrayScale;
    private float baseOffset;
    private float voxelFactor = 0.9f;
    private Vector3 voxelScale;

    private void Start()
    {
        SetupBoard();
    }

    private void SetupBoard()
    {
        arrayScale = scale * 1.1f;
        baseOffset = scale;
        voxelScale = new Vector3(voxelFactor, voxelFactor, voxelFactor);

        globalState = new CellState[width, depth];

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var go = Instantiate(cellInitialPrebab, new Vector3(x* arrayScale, 0, z* arrayScale), new Quaternion());
                globalState[x, z] = go.GetComponent<CellState>();
            }
        }

    }

}
