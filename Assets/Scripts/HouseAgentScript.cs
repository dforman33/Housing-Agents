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

    [HideInInspector] private Plot3D plot;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public Coordinate3D agentCoordinate;
    [HideInInspector] public byte houseID;
    [HideInInspector] public byte houseTargetSize;
    [HideInInspector] public byte houseCurrentSize;
   
    //[HideInInspector] private Queue<Coordinate3D> agentPositions;

    [HideInInspector] public float scale;
    [HideInInspector] public int maxX;
    [HideInInspector] public int maxY;
    [HideInInspector] public int maxZ;
    [HideInInspector] public int movesToTarget = 0;

    [HideInInspector] public bool isActive;
    [HideInInspector] public bool continuesMoving = false;

    //Agent Enhanced Configurations
    //Prohibit occupation of already occupied cells
    [HideInInspector] public bool prohibitOccupyBuiltCells = true;
    //Penalise the occupation of protected or already occupied cells
    [HideInInspector] public bool penaliseNonEmptyCellsOccupation = true;
    //Penalise the occupation of higher positions
    [HideInInspector] public bool penaliseBuildingHigher = true;
    //Rewards ocuppation on completely empty neighbourhoods
    [HideInInspector] public bool rewardEmptyNeighbourhood = true;
    //Rewards occupation of possitions with access to open air on at least one horizontal face
    [HideInInspector] public bool rewardAccessToAir = true;
    //Rewards occupation of possitions that still allow others access to open air on at least one horizontal face
    [HideInInspector] public bool rewardAccessToAirOfOthers = true;
    //Rewards occupation of possitions inmediately close to green open areas
    [HideInInspector] public bool rewardProximityToGreenAreas = true;
    //Rewards speed to complete target
    [HideInInspector] public bool rewardSpeedMeetingTarget = true;

    //EVENTS
    //OnBoundaryCoordinate is invoked in the Clamp() method.
    public event EventHandler OnBoundaryCoordinate;

    //EVENT SUBSCRIBERS

    /// <summary>
    /// Event listener that provides support to add negative rewards when the agent reaches the boundary of the plot.
    /// </summary>
    private void Agent_OnBoundaryCoordinate(object sender, EventArgs e) { AddReward(-2f); }

    /// <summary>
    /// Configures the amount of rewards applied to the agent given the training configurations.
    /// </summary>
    public void UpdateTrainConfig(bool prohibitOccupyBuiltCells, bool penaliseNonEmptyCellsOccupation, bool penaliseBuildingHigher, bool rewardEmptyNeighbourhood, bool rewardAccessToAir, bool rewardAccessToAirOfOthers, bool rewardProximityToGreenAreas, bool rewardSpeedMeetingTarget, bool continuesMoving)
    {
        this.prohibitOccupyBuiltCells = prohibitOccupyBuiltCells; 
        this.penaliseNonEmptyCellsOccupation = penaliseNonEmptyCellsOccupation;
        this.penaliseBuildingHigher = penaliseBuildingHigher;
        this.rewardEmptyNeighbourhood = rewardEmptyNeighbourhood;
        this.rewardAccessToAir = rewardAccessToAir;
        this.rewardAccessToAirOfOthers = rewardAccessToAirOfOthers;
        this.rewardProximityToGreenAreas = rewardProximityToGreenAreas;
        this.rewardSpeedMeetingTarget = rewardSpeedMeetingTarget;
        this.continuesMoving = continuesMoving;
    }



    /// <summary>
    /// The agent remains instantiated but the position is set randomly and all the occupations removed.
    /// </summary>
    public void AgentRestartEpisode(int[,] heightMap, Plot3D plot)
    {
        this.plot = plot;
        this.heightMap = plot.heightMap;
        houseCurrentSize = 0;

        //agentPositions.Clear();

        maxX = plot.width;
        maxY = plot.height;
        maxZ = plot.depth;
        scale = plot.scale;

        RandomMove();
        isActive = true;
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

        this.plot = plot;
        this.houseID = houseID;
        this.houseTargetSize = houseTargetSize;
        this.heightMap = plot.heightMap;
        houseCurrentSize = 0;

        //agentPositions = new Queue<Coordinate3D>();

        maxX = plot.width;
        maxY = plot.height;
        maxZ = plot.depth;
        scale = plot.scale;
        
        RandomMove();
        isActive = true;

    }

    /// <summary>
    /// The house agent occupies the cell at the current position. 
    /// </summary>
    private void OccupyCell()
    {
        plot.OccupyCell(agentCoordinate, houseID);
        //agentPositions.Enqueue(agentCoordinate);
        if (rewardSpeedMeetingTarget) AddReward(0.1f);
    }

    /// <summary>
    /// NOT USED IN THIS COMPUTATION YET. 
    /// The house agent deoccupies the cell at the first position. 
    /// </summary>
    private void DeoccupyCell()
    {
        //plot.OccupyCell(agentPositions.Peek(), 253); // 253 = OPEN AIR CELL
        //agentPositions.Dequeue();
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
        transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
        movesToTarget++;
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
        houseCurrentSize = (byte)plot.EqualIDsCount(houseID);
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
        }
        return isWithinHeight;
    }

    private void InstantiateFailedOccupation()
    {
        CellStateController controller = FindObjectOfType<CellStateController>();
        float timeAlive = 1.0f;
        GameObject failedObject = Instantiate(controller.FailedOccupation, Vector3.zero, Quaternion.identity);
        failedObject.transform.localPosition = Navigation.GetPosition(agentCoordinate, scale);
        Destroy(failedObject, timeAlive);
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

    public override void CollectObservations(VectorSensor sensor)
    {
        //observe the size of the array
        sensor.AddObservation(maxX - 1);
        sensor.AddObservation(maxY - 1);
        sensor.AddObservation(maxZ - 1);

        //observe the coordinate within the array
        sensor.AddObservation(agentCoordinate.X);
        sensor.AddObservation(agentCoordinate.Y);
        sensor.AddObservation(agentCoordinate.Z);

        //observe what is around the agent at any given moment
        sensor.AddObservation(GetNeighborhoodValue()[0]); //EMPTY - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[1]); //OCCUPIED - binary encoding of positions
        sensor.AddObservation(GetNeighborhoodValue()[2]); //OPEN SPACE - binary encoding of positions

        //Observe the neighbourhood of cells around this coordinate
        sensor.AddObservation(ReadSelfNeighbor());

        //Observe the cell type at the current coordinate
        sensor.AddObservation((int)plot.CellInCoord(agentCoordinate).cellType);

        //Observe if the agent has already occupied the cell at the current coordinate
        sensor.AddObservation((int)plot.CellInCoord(agentCoordinate).playerID == houseID? 1: 0);

        //Observe the horizontal neighbourhood to understand if there are occupied cells
        sensor.AddObservation(plot.ReadSqrHorizNeighbors(agentCoordinate));

        //observe the size of the agent house
        sensor.AddObservation(plot.EqualIDsCount(houseID));

        //observe the agent house target size
        sensor.AddObservation(houseTargetSize);

        //observe the type of cell casting a shadow on
        sensor.AddObservation(ShadowingCellType());

        //observe the type of cell casting a shadow on
        sensor.AddObservation(IsWithinHeight()? 1 : 0);

        //Observe if the agent is precluding access to open air to its inmediate neighbourhood
        sensor.AddObservation(plot.IsHorizNeighbourhoodPacked(agentCoordinate));

        //TOTAL OBSERVATIONS = 18
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        //Action - choose one of the 7 possible directions (the last means iddle)
        int directionAction = actions.DiscreteActions[0];
        //Action - decide whether to occupy the position or not
        int occupyAction = actions.DiscreteActions[1];

        houseCurrentSize = (byte)plot.EqualIDsCount(houseID);

        if (houseCurrentSize < houseTargetSize)
        {
            isActive = true;
            //after it has completed the housing programme stops moving
            MoveOne(directionAction);

            if (occupyAction == 1)
            {
                ///Following the same order as the observations 
                ///Prepare some parameters to decide rewards
                
                int emptyNeighbours = plot.GetNeighborhoodValue(agentCoordinate)[0];
                int occupiedNeighbours = plot.GetNeighborhoodValue(agentCoordinate)[1];
                int openProtectedNeighbours = plot.GetNeighborhoodValue(agentCoordinate)[2];
                int selfNeighbourhood = ReadSelfNeighbor();
                int previousCellType = (int)plot.CellInCoord(agentCoordinate).cellType;
                bool positionAlreadyOccupiedByAgent = plot.CellInCoord(agentCoordinate).playerID == houseID ? true : false;
                int horizNeighbourhood = plot.ReadSqrHorizNeighbors(agentCoordinate);
                int previousSize = houseCurrentSize;
                int shadowValue = plot.ShadowingAnotherCell(agentCoordinate);
                bool isWithinHeight = IsWithinHeight();
                

                ///NEGATIVE REWARDS
                ///if shadow on protected ground floor space
                if (shadowValue < 0)
                {
                    AddReward(-2f);
                    InstantiateFailedOccupation();
                    return;
                }

                if (previousCellType == (int)CellType.OPENAIR)
                {
                    if(penaliseNonEmptyCellsOccupation) AddReward(-2f);
                    InstantiateFailedOccupation();
                    return;
                }

                if(previousCellType == (int)CellType.OCCUPIED)
                {
                    if (penaliseNonEmptyCellsOccupation) AddReward(-2f);
                    if (prohibitOccupyBuiltCells)
                    {
                        InstantiateFailedOccupation();
                        return;
                    }
                }

                ///If not shadowing a protected space
                OccupyCell();
                ///UpdateObservations();
                UpdateCurrentSize();



                ///POSITIVE REWARDS
                //if (emptyNeighbours == 0 && rewardEmptyNeighbourhood) AddReward(0.1f);
                if (openProtectedNeighbours > 0 && rewardProximityToGreenAreas) AddReward(1f);
                if (previousSize < houseCurrentSize) AddReward(0.1f);
                //AddReward( 2 / houseTargetSize);

                ///NEGATIVE REWARDS
                if (!isWithinHeight) AddReward(-2f);
                if (occupiedNeighbours == 63) AddReward(-1f);
                if (selfNeighbourhood == 0 && houseCurrentSize > 0) AddReward(-2f);
                if (positionAlreadyOccupiedByAgent) AddReward(-0.5f);
                if (horizNeighbourhood == 15 && rewardAccessToAir) AddReward(-2f);
                if (plot.IsHorizNeighbourhoodPacked(agentCoordinate) > 0 && rewardAccessToAirOfOthers) AddReward(-2f);

                if(penaliseBuildingHigher) AddReward(-0.2f * agentCoordinate.Y); //small negative reward for building higher
            }
        } 
        else if (houseCurrentSize == houseTargetSize && isActive)
        {
            isActive = false;
            //if(rewardSpeedMeetingTarget) AddReward(10 * houseTargetSize / (float) movesToTarget );
        }

        else if (houseCurrentSize == houseTargetSize && !isActive)
        {
            //Code here should address continously moving agents
            //Not implemented yet.
        }

        else if (houseCurrentSize > houseTargetSize) { isActive = false; }
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