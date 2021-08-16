using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using the MLAgents library
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
//using Custom namespace
using Custom;


/// <summary>
/// A house agent to generate the aggregation of one house unit. 
/// It controls the target size and other paramenters to optimise its own benefit.
/// </summary>

public class HouseAgentScript : Agent
{
    //[HideInInspector] public GameObject houseCreatorAgentPrefab;
    [HideInInspector] private Plot3D plot;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public Coordinate3D agentCoordinate;
    [HideInInspector] public byte houseID;
    [HideInInspector] public byte houseTargetSize;
    [HideInInspector] public byte houseCurrentSize;
    
    [HideInInspector] public float scale;
    [HideInInspector] public int maxX;
    [HideInInspector] public int maxY;
    [HideInInspector] public int maxZ;

    [HideInInspector] public bool isActive;
    [HideInInspector] public Observations observations;
   
    
    

    //EVENTS
    //OnBoundaryCoordinate is invoked in the Clamp() method.
    public event EventHandler OnBoundaryCoordinate;
    //OnAboveHeightOccupation is invoked if the player occupies cells above the height determined by the heightmap.
    public event EventHandler OnAboveHeightOccupation;

    //EVENT SUBSCRIBERS
    private void Agent_OnBoundaryCoordinate(object sender, EventArgs e) { AddReward(-1f); }
    private void Agent_OnAboveHeightOccupation(object sender, EventArgs e) { AddReward(-1f); }


    /// <summary>
    /// The agent remains instantiated but the position is set randomly and all the occupations removed.
    /// </summary>
    public void AgentRestartEpisode(int[,] heightMap)
    {
        RandomMove();
        isActive = true;
        UpdateObservations();
        houseCurrentSize = 0;
        this.heightMap = heightMap;
    }


    /// <summary>
    /// At the agent initialization the game object at a random position.
    /// </summary>
    public void AgentInit(byte houseID, byte houseTargetSize, Plot3D plot)
    {
        //Subscribe to events
        OnBoundaryCoordinate += Agent_OnBoundaryCoordinate;
        OnAboveHeightOccupation += Agent_OnAboveHeightOccupation;

        //controller = FindObjectOfType<CellStateController>();
        //houseCreatorAgentPrefab = controller.HouseCreator;

        //houseCreatorAgentPrefab = Instantiate(houseCreatorAgentPrefab, Vector3.zero, Quaternion.identity);
        //houseCreatorAgentPrefab.transform.parent = transform;
        //houseCreatorAgentPrefab.name = $"house_agent_{houseID}";

        this.plot = plot;
        this.houseID = houseID;
        this.houseTargetSize = houseTargetSize;
        this.heightMap = plot.heightMap;
        houseCurrentSize = 0;

        maxX = plot.width;
        maxY = plot.height;
        maxZ = plot.depth;
        scale = plot.scale;
        
        RandomMove();
        isActive = true;
        observations = new Observations { agentID = houseID, currentSize = 0 };
        UpdateObservations();
        
    }


    /// <summary>
    /// The house agent occupies the cell at the current position. 
    /// </summary>
    private void OccupyCell()
    {
        plot.OccupyCell(agentCoordinate, houseID);
        IsWithinHeight();
    }

    /// <summary>
    /// Stores the vector3 position as an agentCoordinate parameter. 
    /// A clamp function ensures that the agent remains inside the edges of the array.
    /// </summary>
    /// <param name="position">A world position translated into a coordinate within the array.</param>
    void UpdateCoordinate(Vector3 position)
    {
        agentCoordinate = Navigation.GetCoordinates3D(position, scale);
        ClampCoordinate();
    }

    /// <summary>
    /// Generates a random move for the house agent. 
    /// </summary>
    private void RandomMove()
    {
        Vector3 pos = Navigation.GetPosition(UnityEngine.Random.Range(0, maxX), UnityEngine.Random.Range(0, maxY), UnityEngine.Random.Range(0, maxZ), scale);
        UpdateCoordinate(pos);
        transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
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
        else { return; }
        //houseCreatorAgentPrefab.transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
        transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
    }

    /// <summary>
    /// Ensures that no move falls outside the three dimensional grid that defines the plot3D.
    /// It also makes sure that the agent does not occupy the edge conditions to facilitate the reading of the cell's neighborhood.
    /// </summary>
    private void ClampCoordinate()
    {
        bool isClamped = false;
        agentCoordinate.ClampCoordinate(1, maxX - 2, 1, maxY - 2, 1, maxZ - 2, out isClamped);
        if (isClamped) OnBoundaryCoordinate?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Calculates the total number of cells occupied by the agent house. 
    /// </summary>
    private void UpdateCurrentSize()
    {
        observations.currentSize = plot.EqualIDsCount(houseID);
        houseCurrentSize = (byte)observations.currentSize;
        Debug.Log("Player " + houseID + " current size " + houseCurrentSize);
    }

    /// <summary>
    /// Explores the position of the agent ageinst the heightmap.
    /// </summary>
    /// <returns>An integer that is the binary value of the square neighborhood of cells with the same ID.</returns>
    private bool IsWithinHeight()
    {
        bool isWithinHeight;
        if (agentCoordinate.Y <= heightMap[agentCoordinate.X, agentCoordinate.Z])
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

    public void UpdateObservations()
    {
        observations.currentCellType = (int)plot.CellInCoord(agentCoordinate).cellType;
        Debug.Log($"observations.currentCellType: {observations.currentCellType}");
        observations.neighborhoodValues = plot.GetNeighborhoodValue(agentCoordinate);
        observations.selfNeighbourhood = plot.ReadSelfNeighbor(agentCoordinate, houseID);
        observations.horizNeighbourhood = plot.ReadSqrHorizNeighbors(agentCoordinate);
        observations.currentSize = plot.EqualIDsCount(houseID);
        observations.shadowOnCellType = plot.ShadowingAnotherCell(agentCoordinate);

        houseCurrentSize = (byte)observations.currentSize;
    }

    private void InstantiateFailedOccupation()
    {
        CellStateController controller = FindObjectOfType<CellStateController>();
        float timeAlive = 1.0f;
        GameObject failedObject = Instantiate(controller.FailedOccupation, Vector3.zero, Quaternion.identity);
        failedObject.transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
        Destroy(failedObject, timeAlive);
    }

    /*TEST
     * */

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

    public int ReadSelfNeighbor()
    {
        return plot.ReadSelfNeighbor(agentCoordinate, houseID);
    }

    private int ShadowingCellType()
    {
        return plot.ShadowingAnotherCell(agentCoordinate);
    }


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

    //public override void OnEpisodeBegin()
    //{
    //    AgentRestartEpisode(heightMap);
    //}

    public override void CollectObservations(VectorSensor sensor)
    {
        UpdateObservations();

        //observe the size of the array
        sensor.AddObservation(maxX);
        sensor.AddObservation(maxY);
        sensor.AddObservation(maxZ);

        //observe the coordinate within the array
        sensor.AddObservation(agentCoordinate.X);
        sensor.AddObservation(agentCoordinate.Y);
        sensor.AddObservation(agentCoordinate.Z);

        //Observe the cell at the current coordinate
        sensor.AddObservation((int)plot.CellInCoord(agentCoordinate).cellType);

        //observe what is around the agent at any given moment
        //sensor.AddObservation(observations.neighborhoodValues[0]); //EMPTY - binary encoding of positions
        //sensor.AddObservation(observations.neighborhoodValues[1]); //OCCUPIED - binary encoding of positions
        //sensor.AddObservation(observations.neighborhoodValues[2]); //OPEN SPACE - binary encoding of positions

        sensor.AddObservation(GetNeighborhoodValue()[0]); //EMPTY - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[1]); //OCCUPIED - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[2]); //OPEN SPACE - binary encoding of positions

        //Observe the neighbourhood of cells around this coordinate
        //sensor.AddObservation(observations.selfNeighbourhood);
        sensor.AddObservation(ReadSelfNeighbor());

        //Observe the horizontal neighbourhood to understand if there are occupied cells
        //sensor.AddObservation(observations.horizNeighbourhood);
        sensor.AddObservation(plot.ReadSqrHorizNeighbors(agentCoordinate));

        //observe the size of the agent house
        //sensor.AddObservation(observations.currentSize);
        sensor.AddObservation(plot.EqualIDsCount(houseID));

        //observe the agent house target size
        sensor.AddObservation(houseTargetSize);

        //observe the type of cell casting a shadow on
        //sensor.AddObservation(observations.shadowOnCellType);
        sensor.AddObservation(ShadowingCellType());

        //TOTAL OBSERVATIONS = 15
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        if (isActive)
        {

            UpdateObservations();

            int housePreviousSize = houseCurrentSize;

            //Action - choose one of the 7 possible directions (the last means iddle)
            int directionAction = actions.DiscreteActions[0];
            MoveOne(directionAction);

            //Action - decide whether to occupy the position or not
            int occupyAction = actions.DiscreteActions[1];
            if (occupyAction == 1)
            {
                //Determine the type of cell we are casting a shadow on
                int shadowValue = observations.shadowOnCellType;
                if (shadowValue < 0)
                {
                    AddReward(-2f);
                    InstantiateFailedOccupation();
                }
                else
                {
                    //Previous cell type at the current coordinate
                    int previousCellType = observations.currentCellType;


                    OccupyCell();
                    UpdateObservations();
                    UpdateCurrentSize();

                    //ADD REWARDS BASED ON THE ACTIONS' RESULTS

                    //Reward according to the previous type of the cell just occupied
                    if (previousCellType == (int)CellType.OPENAIR) AddReward(-2f);
                    if (previousCellType == (int)CellType.OCCUPIED) AddReward(-1.5f);

                    //Check the height policy 
                    IsWithinHeight();

                    //Reward positions that are not in isolation with regards to the player other positions

                    if (observations.selfNeighbourhood == 0 && houseCurrentSize > 1) AddReward(-1f);
                    else AddReward(0.2f);

                    if (observations.horizNeighbourhood == 15) AddReward(-1);

                    AddReward(-0.1f * agentCoordinate.Y); //small negative reward for building higher

                    //Reward growth and proximity to targeted size
                    if (houseCurrentSize < houseTargetSize && houseCurrentSize > housePreviousSize) AddReward(1f);
                    else if (houseCurrentSize > houseTargetSize) AddReward(-0.2f * (houseTargetSize - houseCurrentSize));
                    else if (houseCurrentSize == houseTargetSize) AddReward(1f);
                }
            }
        }
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