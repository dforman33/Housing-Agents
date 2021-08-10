using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Custom namespace
using Custom;


public class SunCastingPlot : Plot3D
{
    [HideInInspector] public int[,] openGFMap;
    [HideInInspector] public static Vector3 sunReverseDirection = new Vector3(0.1f,1,0.1f);


    private void Awake()
    {

        controller = GetComponent<CellStateController>();
        Camera.main.transform.position = new Vector3(-6f, 15.5f, -6f);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(33, 45, 0));
        SetupBoard();
        heightMap = new HeightMapGen(this, minHeight, maxHeight, 2).heightMap;
        openGFMap = new OpenGFGenerator(this, 2, 50).openGFMap;
        AddSomeOccupied();
    }

    private void Start()
    {
        AddGround();
        DisplayOpenSpace();
    }
    public void DisplayOpenSpace()
    {
        int count = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (openGFMap[x, z] == 1)
                {
                    cells[x, 0, z].UpdateColor(new Color(.95f, .95f, .95f)); //Open Space at Ground Floor
                    count++;
                }

            }
        }
        Debug.Log("Open space created: " + count);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckOpenSpaceOcclussions();
        }
    }

    public void CheckOpenSpaceOcclussions()
    {
        int count = 0;
        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (openGFMap[x, z] == 1)
                {
                    if (RayCast(new Coordinate3D(x, 0, z), sunReverseDirection, "CellOccupied"))
                    {
                        Debug.Log("Cell is occluding Open Space for OS: " + x + "," + z);
                        count++;
                        Vector3 start = Navigation.GetPosition(new Coordinate3D(x, 0, z), scale);
                        Vector3 end = start + sunReverseDirection.normalized * 200;
                        Debug.DrawRay(start, sunReverseDirection,Color.red, 5f);
                        Debug.DrawLine(start, end, Color.blue, 20f);
                    }
                }
            }
        }
        Debug.Log("Collisions found: " + count);
    }

    public bool RayCast(Vector3 position, Vector3 direction, string tagName)
    {
        bool punish = false;
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        float maxDist= (float)(width + height + depth);

        if (Physics.Raycast(ray, out hit, maxDist))
        {
            if (hit.collider.CompareTag(tagName))
                punish = true;
        }
        return punish;
    }

    public bool RayCast(Coordinate3D coord, Vector3 direction, string tagName)
    {
        return RayCast(Navigation.GetPosition(coord, scale), direction, tagName);
    }
}