using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Custom namespace
using Custom;


/// <summary>
/// A house agent to generate the aggregation of one house unit. 
/// It controls the target size and other paramenters to optimise its own benefit.
/// </summary>

public class HouseCreatorAgent : MonoBehaviour
{
    public GameObject houseCreatorAgentPrefab;
    private Coordinate3D agentCoordinate;
    private Plot3D plot;

    [HideInInspector] public byte houseID;
    [HideInInspector] public byte houseTargetSize;
    [HideInInspector] public byte houseCurrentSize;
    
    /// <summary>
    /// At the agent initialization the game object at a random position.
    /// The house agent will loop a maximum of 100 times to attempt to occupy empty cells only. 
    /// If no empty cell is found, at the last attempt it will occupy any cell it is in at that time. 
    /// </summary>
    private void AgentInit(byte houseID, byte targetSize)
    {
        this.houseID = houseID;
        houseTargetSize = targetSize;
        houseCreatorAgentPrefab = Instantiate(this.houseCreatorAgentPrefab, transform.position, Quaternion.identity);
        houseCreatorAgentPrefab.transform.parent = transform;
        houseCreatorAgentPrefab.name = "house_agent_" + houseID;
        RandomMove();

        bool hasOccupiedFirst = false;

        if (!plot.IsCellOccupied(agentCoordinate))
        {
            OccupyCell();
            hasOccupiedFirst = true;
            Debug.Log("Occupied at first attempt");
        }

        else
        {
            int loopCount = 0;
            while (plot.IsCellOccupied(agentCoordinate) && loopCount < 100)
            {
                loopCount++;
                RandomMove();
                if (!plot.IsCellOccupied(agentCoordinate)) 
                { 
                    OccupyCell();
                    hasOccupiedFirst = true;
                    break;
                }
            }
            if(!hasOccupiedFirst) OccupyCell();
            Debug.Log("Times looped to occupy: " + loopCount);
        }
    }

    /// <summary>
    /// If no house target size is provided the value is set to 10 by default. 
    /// </summary>
    /// <param name="houseID">This is the housing agent's unique ID.</param>

    private void AgentInit(byte houseID)
    {
        AgentInit(houseID, 10);
    }
    /// <summary>
    /// If invoked the house agent occupies the cell at the current position. 
    /// </summary>
    private void OccupyCell()
    {
        plot.OccupyCell(agentCoordinate, houseID);
        IsWithinHeight();
    }
    /// <summary>
    /// Stores the coordinate value in the agentCoordinate parameter.
    /// A clamp function ensures that the agent remains inside the edges of the array.
    /// </summary>
    void UpdateCoordinate()
    {
        agentCoordinate = Navigation.GetCoordinates3D(houseCreatorAgentPrefab.transform.localPosition, plot.scale);
        ClampCoordinate();
    }

    /// <summary>
    /// Stores the vector3 position as an agentCoordinate parameter. 
    /// A clamp function ensures that the agent remains inside the edges of the array.
    /// </summary>
    /// <param name="position">A world position translated into a coordinate within the array.</param>
    void UpdateCoordinate(Vector3 position)
    {
        agentCoordinate = Navigation.GetCoordinates3D(position, plot.scale);
        ClampCoordinate();
    }

    /// <summary>
    /// Generates a random move for the house agent. 
    /// </summary>
    private void RandomMove()
    {
        Vector3 pos = Navigation.GetPosition(Random.Range(0, plot.width), Random.Range(0, plot.height), Random.Range(0, plot.depth), plot.scale);
        UpdateCoordinate(pos);
        houseCreatorAgentPrefab.transform.localPosition = Navigation.GetPosition(agentCoordinate, plot.scale);
    }

    /// <summary>
    /// Basic move functionality, inputs six possible integers (between 0 and 5) each corresponding with a possible direction in the space. 
    /// It moves by one cell in the input direction. 
    /// </summary>
    /// <param name="directionID">One of the possible six directions, to return a valid move must be between 0 and 5.</param>
    private void MoveOne(int directionID)
    {
        if (directionID >= 0 && directionID < Navigation.directions3D.Length)
        {
            agentCoordinate += Navigation.directions3D[directionID];
            ClampCoordinate();
        }
        else
            return;
        houseCreatorAgentPrefab.transform.localPosition = Navigation.GetPosition(agentCoordinate, plot.scale);
    }

    /// <summary>
    /// Ensures that no move falls outside the three dimensional grid that defines the plot3D.
    /// It also makes sure that the agent does not occupy the edge conditions to facilitate the reading of the cell's neighborhood.
    /// </summary>
    private void ClampCoordinate()
    {
        agentCoordinate.ClampCoordinate(1, plot.width - 2, 1, plot.depth - 2, 1, plot.height - 2);
    }

    /// <summary>
    /// Calculates the total number of cells occupied by the agent house. 
    /// </summary>
    private void UpdateCurrentSize()
    {
        byte count = 0;
        foreach (var cell in plot.cells)
        {
            if (cell.playerID == houseID) count++;
        }
        houseCurrentSize = count;
        Debug.Log("Player " + houseID + " current size " + houseCurrentSize);
    }


    /// <summary>
    /// returns an array of size three invoking the GetNeighborhoodValue() method within the plot3D class.
    /// </summary>
    /// <returns> 
    /// [0] returns the empty positions. [1] returns the occupied positions. [2] returns the open space positions.</returns>
    private int[] GetNeighborhoodValue()
    {
        int [] result = plot.GetNeighborhoodValue(agentCoordinate);
        Debug.Log("Neighborhood value. Empty: " + result[0] + " ,Occupied: "+ result[1] + " ,Open Space: "+ result[2]);
        return result;
    }

    private int ReadSelfNeighbor()
    {
        int result = plot.ReadSelfNeighbor(agentCoordinate, houseID);
        Debug.Log("Neighborhood self value: " + result);
        return result;
    }

    private bool IsWithinHeight()
    {
        bool isWithinHeight;
        if (agentCoordinate.Y <= plot.heightMap[agentCoordinate.X, agentCoordinate.Z]) isWithinHeight = true;
        else
        {
            isWithinHeight = false;
            Debug.Log("Too high, consider a negative reward");
        }
        return isWithinHeight;
    }


    private void Start()
    {
        plot = GetComponent<Plot3D>();
        AgentInit(1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) RandomMove();

        if (Input.GetKeyDown(KeyCode.T))
        {
            OccupyCell();
            UpdateCurrentSize();
            GetNeighborhoodValue();
            ReadSelfNeighbor();
        }

        if (Input.GetKeyDown(KeyCode.W)) MoveOne((int)DirectionChoice.Front);
        if (Input.GetKeyDown(KeyCode.S)) MoveOne((int)DirectionChoice.Back);
        if (Input.GetKeyDown(KeyCode.A)) MoveOne((int)DirectionChoice.Left);
        if (Input.GetKeyDown(KeyCode.D)) MoveOne((int)DirectionChoice.Right);
        if (Input.GetKeyDown(KeyCode.E)) MoveOne((int)DirectionChoice.Up);
        if (Input.GetKeyDown(KeyCode.Q)) MoveOne((int)DirectionChoice.Down);

    }
}