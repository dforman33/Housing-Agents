using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Custom namespace
using Custom;


public class HouseCreatorAgent : MonoBehaviour
{
    public GameObject houseCreatorAgentPrefab;
    private Coordinate3D agentCoordinate;
    private Plot3D plot;

    [HideInInspector] public byte houseID;
    [HideInInspector] public byte houseTargetSize;
    [HideInInspector] public byte houseCurrentSize;
    
    [SerializeField] private float MoveTime = 0.05f;
    [HideInInspector] private float _timeUntilMove;

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
    }
    /// <summary>
    /// Stores the coordinate value in the agentCoordinate parameter. 
    /// </summary>
    void UpdateCoordinate()
    {
        agentCoordinate = Navigation.GetCoordinates3D(houseCreatorAgentPrefab.transform.localPosition, plot.scale);
        ClampCoordinate();
    }

    /// <summary>
    /// Generates a random move for the house agent. 
    /// </summary>
    private void RandomMove()
    {
        Vector3 pos = Navigation.GetPosition(Random.Range(0, plot.width), Random.Range(0, plot.height), Random.Range(0, plot.depth), plot.scale);
        UpdateCoordinate();
        ClampCoordinate();
        houseCreatorAgentPrefab.transform.localPosition = Navigation.GetPosition(agentCoordinate, plot.scale);
    }

    /// <summary>
    /// Basic move functionality, inputs six possible integers (between 0 and 5) each corresponding with a possible direction in the space. 
    /// It moves by one cell in the input direction. 
    /// </summary>

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

    private void ClampCoordinate()
    {
        agentCoordinate.ClampCoordinate(1, plot.width - 2, 1, plot.depth - 2, 1, plot.height - 2);

    }

    /// <summary>
    /// Provides a slower move sensitivity to facilitate human player control. 
    /// </summary>

    void SlowMove(int directionID)
    {
        _timeUntilMove -= Time.deltaTime;
        if (_timeUntilMove > 0.0f)
        {
            return;
        }
        MoveOne(directionID);
        _timeUntilMove = MoveTime;
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