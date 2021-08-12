using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using the MLAgents library
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//using Custom namespace
using Custom;


/// <summary>
/// A house agent to generate the aggregation of one house unit. 
/// It controls the target size and other paramenters to optimise its own benefit.
/// </summary>

public class HouseAgentScene06 : Agent
{
    public GameObject houseCreatorAgentPrefab;
    private Coordinate3D agentCoordinate;
    [HideInInspector] public Plot3D plot;

    [HideInInspector] public byte houseID;
    [HideInInspector] public byte houseTargetSize;
    [HideInInspector] public byte houseCurrentSize;

    private static System.Random random;

    //EVENTS

    //OnBoundaryCoordinate is invoked in the Clamp() method.
    public event EventHandler OnBoundaryCoordinate;
    //OnAboveHeightOccupation is invoked if the player occupies cells above the height determined by the heightmap.
    public event EventHandler OnAboveHeightOccupation;
    
    //EVENTS


    /// <summary>
    /// At the agent initialization the game object at a random position.
    /// </summary>
    private void AgentInit(byte houseID, byte targetSize)
    {
        this.houseID = houseID;
        houseTargetSize = targetSize;
        if (houseCreatorAgentPrefab == null)
        {
            houseCreatorAgentPrefab = Instantiate(houseCreatorAgentPrefab, Vector3.zero, Quaternion.identity);
            houseCreatorAgentPrefab.transform.parent = transform;
            houseCreatorAgentPrefab.name = "house_agent_" + houseID;
        }

        RandomMove();
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
        //IsWithinHeight();
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
        Vector3 pos = Navigation.GetPosition(UnityEngine.Random.Range(0, plot.width), UnityEngine.Random.Range(0, plot.height), UnityEngine.Random.Range(0, plot.depth), plot.scale);
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
        bool isClamped = false;
        agentCoordinate.ClampCoordinate(1, plot.width - 2, 1, plot.height - 2, 1, plot.depth - 2, out isClamped);
        if (isClamped) OnBoundaryCoordinate?.Invoke(this, EventArgs.Empty);
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
        int[] result = plot.GetNeighborhoodValue(agentCoordinate);
        return result;
    }
    /// <summary>
    /// Returns binary number of neighbors with the same playerID.
    /// </summary>
    /// <returns>An integer that is the binary value of the square neighborhood of cells with the same ID.</returns>
    private int ReadSelfNeighbor()
    {
        return plot.ReadSelfNeighbor(agentCoordinate, houseID);
    }

    /// <summary>
    /// Explores the position of the agent ageinst the heightmap.
    /// </summary>
    /// <returns>An integer that is the binary value of the square neighborhood of cells with the same ID.</returns>
    private bool IsWithinHeight()
    {
        bool isWithinHeight;
        if (agentCoordinate.Y <= plot.heightMap[agentCoordinate.X, agentCoordinate.Z])
        {
            isWithinHeight = true;
        }
            
        else
        {
            isWithinHeight = false;
            OnAboveHeightOccupation?.Invoke(this, EventArgs.Empty);
        }
        return isWithinHeight;
    }

    private void Awake()
    {
        //random = new System.Random();

        //Initialise plot
        plot = GetComponent<Plot3D>();
        plot.SetupBoard();

        //Get agent prefab
        houseCreatorAgentPrefab = plot.controller.HouseCreator;
        plot.AddPlotConstraints();

        //Subscribe to events
        OnBoundaryCoordinate += Agent_OnBoundaryCoordinate;
        OnAboveHeightOccupation += Agent_OnAboveHeightOccupation;
    }


    private void Agent_OnBoundaryCoordinate(object sender, EventArgs e) { Debug.Log("Boundary reached"); AddReward(-1f); }
    private void Agent_OnAboveHeightOccupation(object sender, EventArgs e) { Debug.Log("Height Exceeded"); AddReward(-1f); }


    /*
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
       * 
       *           UNITY ML AGENTS 
       * 
       *            PLACED BELOW
       * 
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    */

    public override void OnEpisodeBegin()
    {
        plot.AddPlotConstraints();
        AgentInit(1);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //observe the size of the array
        sensor.AddObservation(plot.width);
        sensor.AddObservation(plot.depth);
        sensor.AddObservation(plot.height);

        //observe the coordinate within the array
        sensor.AddObservation(agentCoordinate.X);
        sensor.AddObservation(agentCoordinate.Y);
        sensor.AddObservation(agentCoordinate.Z);

        //Observe the cell at the currnet coordinate
        sensor.AddObservation((int)plot.CellInCoord(agentCoordinate).cellType);

        //observe what is around the agent at any given moment
        sensor.AddObservation(GetNeighborhoodValue()[0]); //EMPTY - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[1]); //OCCUPIED - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[2]); //OPEN SPACE - binary encoding of positions

        //observe the size of the agent
        sensor.AddObservation(houseCurrentSize);

        //TOTAL OBSERVATIONS = 11
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int count = 0;
        int housePreviousSize = houseCurrentSize;

        //Action - choose one of the 7 possible directions (the last means iddle)
        int directionAction = actions.DiscreteActions[0];
        MoveOne(directionAction);

        //Action - decide whether to occupy the position or not
        int occupyAction = actions.DiscreteActions[1];
        if (occupyAction == 1)
        {
            CellType previousCellType = plot.CellInCoord(agentCoordinate).cellType;
            OccupyCell();
            UpdateCurrentSize();

            //ADD REWARDS BASED ON THE ACTIONS' RESULTS

            //Reward according to the previous type of the cell just occupied
            if (previousCellType == CellType.OPENSPACE) AddReward(-2f);
            if (previousCellType == CellType.OCCUPIED) AddReward(-0.1f);

            //Reward positions that are not in isolation with regards to the player other positions
            IsWithinHeight(); //Checks for the height policy stored in the heightMap
            if (ReadSelfNeighbor() == 0 && houseCurrentSize > 1) AddReward(-1f);
            else AddReward(0.2f);
            AddReward(-0.1f * agentCoordinate.Y); //small negative reward to build higher

            //Reward growth and proximity to targeted size
            if (houseCurrentSize < houseTargetSize && houseCurrentSize > housePreviousSize) AddReward(1f);
            if (houseTargetSize == houseCurrentSize)
            {
                AddReward(5f);
                EndEpisode();
            }

            if (houseCurrentSize > houseTargetSize) AddReward(-0.25f);
            if (houseCurrentSize == houseTargetSize + 2) EndEpisode();
        }

        count++;
        AddReward(-0.1f);
        if (count > 30) EndEpisode();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        int directionAction = 6;

        if (Input.GetKey(KeyCode.W)) directionAction = 0;
        else if (Input.GetKey(KeyCode.S)) directionAction = 1;
        else if (Input.GetKey(KeyCode.A)) directionAction = 2;
        else if (Input.GetKey(KeyCode.D)) directionAction = 3;
        else if (Input.GetKey(KeyCode.E)) directionAction = 4;
        else if (Input.GetKey(KeyCode.Q)) directionAction = 5;


        int occupyAction = (Input.GetKey(KeyCode.T)) ? 1 : 0;

        discreteActions[0] = directionAction;
        discreteActions[1] = occupyAction;

    }



}