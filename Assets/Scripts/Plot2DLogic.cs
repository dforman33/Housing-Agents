using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Custom namespaces
using Custom;


public class Plot2DLogic : MonoBehaviour
{
    //Custom values that appear in the hierarchy

    [Header("Plot Setting Up")]
    [SerializeField] private int width = 10; //x length
    [SerializeField] private int depth = 10; //z length
    [SerializeField] private float scale = 1;
    [SerializeField] private Vector3 plotOrigin = new Vector3(0, 0, 0);

    [Header("Player Prefabs")]
    [SerializeField] private GameObject objectTypeA;
    [SerializeField] private GameObject objectTypeB;
    [SerializeField] private GameObject objectTypeC;
    [SerializeField] private GameObject objectTypeD;

    //values to keep the visual representation of the voxels smaller to clearly see gaps
    private float arrayScale = 1;
    private float voxelFactor = 0.95f;
    private Vector3 voxelScale;
    private float baseOffset;


    //The instance of the plot class
    private Plot2D plot;

    private void Awake()
    {
        Camera.main.transform.position = new Vector3(9f, 20f, 9.5f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        voxelScale = new Vector3(voxelFactor, voxelFactor, voxelFactor);
        arrayScale = scale * 2;
        baseOffset = 0.5f * arrayScale;

    }

    private void Start()
    {
        plot = new Plot2D(width, depth, plotOrigin); //if a 2D aggregation a maximum height of 1 is automatically generated
        DrawPlot();
    }

    private void Update()
    {
        bool remove = false;
        bool isWithinPlot = false;
        Coordinate2D plotCoord = Navigation.CheckCollisionOnPlot(plotOrigin, arrayScale, width, depth, out remove, out isWithinPlot);


        if (isWithinPlot && !remove)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Cell protected at " + plotCoord.X + "," + plotCoord.Z);
                plot.UpdatePlot2D(plotCoord, 255);
                ClearObjects();
                DrawPlot();
            }
            else
            {
                Debug.Log("Cell occupied at " + plotCoord.X + "," + plotCoord.Z);
                plot.UpdatePlot2D(plotCoord, 1);
                ClearObjects();
                DrawPlot();
            }
        }
        else if (isWithinPlot && remove)
        {
            Debug.Log("Cell removed at " + plotCoord.X + "," + plotCoord.Z);
            plot.UpdatePlot2D(plotCoord, 0);
            ClearObjects();
            DrawPlot();
        }

    }
    
    private void DrawPlot()
    {
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (plot.cells[x, z].state == 0)
                {
                    Vector3 pos = new Vector3(x * arrayScale, 0, z * arrayScale);
                    pos += plotOrigin;
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    obj.transform.localScale = ((voxelScale * arrayScale) - new Vector3(0, 0.99f, 0));
                    obj.transform.position = pos;
                    //obj.GetComponent<Renderer>().material.color = Extensions.HexToRGB("#03045e");
                }

                else if (plot.cells[x, z].state == 1)
                {
                    Vector3 pos = new Vector3(x * arrayScale, baseOffset, z * arrayScale);
                    pos += plotOrigin;
                    GameObject obj = GameObject.Instantiate(objectTypeA, pos, Quaternion.identity);
                    obj.transform.localScale = voxelScale;
                }

                else if (plot.cells[x, z].state == 255)
                {
                    Vector3 pos = new Vector3(x * arrayScale, 0, z * arrayScale);
                    pos += plotOrigin;
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    obj.transform.localScale = ((voxelScale * arrayScale) - new Vector3(0, 0.99f, 0));
                    obj.transform.position = pos;
                    obj.GetComponent<Renderer>().material.color = Color.grey;

                }
            }
        }
    }
    private void ClearObjects()
    {
        foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
        {
            if (o.name.Contains("Cube") ||
                o.name.Contains("Octa"))

                Destroy(o);
        }
    }


}
