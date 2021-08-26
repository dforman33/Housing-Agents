using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using the MLAgents library
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
//using Custom namespace
using Custom;


/// <summary>
/// A house agent to generate the aggregation of one house unit. 
/// It controls the target size and other paramenters to optimise its own benefit.
/// </summary>

public class Scene08Environment : MonoBehaviour
{
    [HideInInspector] public Plot3D plot;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public CellStateController controller;
    [HideInInspector] public int occupationTarget;
    [HideInInspector] public int occupationCount = 0;
    [HideInInspector] public int availableCellsCount = 0;
    [HideInInspector] public int emptyCellsCount = 0;
    [HideInInspector] public int occupationFootprint = 0;
    [HideInInspector] public int environmentResets = 0;
    [HideInInspector] public int activePlayers = 0;
    [HideInInspector] public int inactivePlayers = 0;
    [HideInInspector] public int numberOfPlayers = 0;
    [HideInInspector] public int envMoveSteps = 0;
    [HideInInspector] public float occupationRateLimit;

    [SerializeField] private int minNumberOfPlayers = 4;
    [SerializeField] public int MaxEnvironmentSteps = 3600;
    [SerializeField] private int playerTimer = 180;
    [SerializeField] private int playerMovesLimit = 15;
    [SerializeField] private bool reachStableStateOn = false;
    [SerializeField] private float occupationRateToAddPlayer;
    

    private int playerMoves = 0;
    private int currentPlayer;

    [HideInInspector] public SimpleMultiAgentGroup multiAgentGroup;
    [HideInInspector] public List<HouseAgentScript> AgentsList = new List<HouseAgentScript>();



    /// METHODS

    /// <summary>
    /// Moves to the next player within the list of players.
    /// </summary>
    private void UpdatePlayer()
    {
        currentPlayer++;
        if (currentPlayer > numberOfPlayers) currentPlayer = 1;

        int active = 0;
        int inactive = 0;

        foreach (var player in AgentsList)
        {
            if (player.isActive) active++;
            else inactive++;
        }
        activePlayers = active;
        inactivePlayers = inactive;
    }
    /// <summary>
    /// Updates the total number of occupations considering all agents and players.
    /// </summary>
    private void UpdateOccupationCountAndFootprint()
    {
        occupationCount = plot.CellTypeCount(CellType.OCCUPIED);
        occupationFootprint = plot.CellTypeFootprint(CellType.OCCUPIED);
        emptyCellsCount = plot.CellTypeCount(CellType.EMPTY);
    }

    /// <summary>
    /// Called when the whole scene needs to be cleaned and values reset to the initial state.
    /// </summary>
    public void ResetScene()
    {
        environmentResets++;

        int newWidth = UnityEngine.Random.Range(8, 20);
        int newHeight = UnityEngine.Random.Range(8, 20);
        int newDepth = UnityEngine.Random.Range(8, 20);

        // This should provide an open space threshold ranging between 50 and 80, meaning 50 and 20% of open space
        int openSpaceThreshold = 50 + 10 * UnityEngine.Random.Range(0, 4);
        plot.ResetBoard(newWidth, newHeight, newDepth, openSpaceThreshold);

        List<HouseAgentScript> tempAgentsList = new List<HouseAgentScript>();
        for (int i = 0; i < AgentsList.Count; i++)
        {
            AgentsList[i].AgentRestartEpisode(plot.heightMap, plot);

            if (i < minNumberOfPlayers)
            {
                tempAgentsList.Add(AgentsList[i]);
            }
            else
            {
                multiAgentGroup.UnregisterAgent(AgentsList[i]);
                Destroy(AgentsList[i].gameObject);
            }
        }


        ParameterSetup();

        AgentsList.Clear();
        AgentsList.AddRange(tempAgentsList);


        foreach (var agent in AgentsList)
        {
            if (agent.isActive) activePlayers++;
            else inactivePlayers++;
            
            occupationTarget += agent.houseTargetSize;
        }

        foreach (var agent in AgentsList)
        {
            Debug.Log($"Agent {agent.houseID} target size: {agent.houseTargetSize}");
        }
    }

    void ParameterSetup()
    {
        occupationRateLimit = UnityEngine.Random.Range(0.15f, 0.45f);
        occupationRateToAddPlayer = occupationRateLimit - 0.05f;

        occupationTarget = 0;
        activePlayers = 0;
        inactivePlayers = 0;
        envMoveSteps = 0;
        playerMoves = 0;
        playerTimer = 0;
        occupationCount = 0;
        numberOfPlayers = minNumberOfPlayers;
        currentPlayer = 1;
        availableCellsCount = plot.CellTypeCount(CellType.EMPTY);
        emptyCellsCount = availableCellsCount;
    }

    /// <summary>
    /// Called when the whole scene needs to be cleaned and values reset to the initial state.
    /// </summary>
    public void AddAgent()
    {
        numberOfPlayers++;
        GameObject instance = Instantiate<GameObject>(controller.HouseCreator, Vector3.zero + transform.localPosition, Quaternion.identity);
        instance.transform.parent = transform;
        instance.name = $"houseID-{numberOfPlayers}";
        HouseAgentScript houseAgent = instance.GetComponent<HouseAgentScript>();
        houseAgent.AgentInit((byte)numberOfPlayers, (byte)UnityEngine.Random.Range(6, 12), this.plot);
        AgentsList.Add(houseAgent);
        multiAgentGroup.RegisterAgent(AgentsList[numberOfPlayers - 1]);
        occupationTarget += houseAgent.houseTargetSize;
        Debug.Log($"Player {numberOfPlayers} added, occupation target now is {occupationTarget}.");

    }


    /*
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
       * 
       *           UNITY MONOBEHAVIOUR METHODS 
       * 
       *                  PLACED BELOW
       * 
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    /// --- /// --- /// --- /// --- /// --- /// --- /// --- /// --- ///
    */

    // ACTION SUBSCRIBER
    private void Plot_OnOccupyCell(int playerID)
    {
        UpdatePlayer();
        UpdateOccupationCountAndFootprint();
        multiAgentGroup.AddGroupReward((float)occupationCount / (float)occupationTarget);
        multiAgentGroup.AddGroupReward((float)occupationCount / (float)occupationFootprint);
        playerTimer = 0;
    }

    private void Awake()
    {
        controller = GetComponent<CellStateController>();
        plot = GetComponent<Plot3D>();
        plot.SetupBoard();

        //subscribe to actions and events
        plot.OnOccupyCell += Plot_OnOccupyCell;
        //Multi-agent config, part of ML-Agents and parallel list of housing agents
        multiAgentGroup = new SimpleMultiAgentGroup();
        AgentsList = new List<HouseAgentScript>();

    }

    private void Start()
    {
        //Add constraints
        int openSpaceThreshold = 75;
        plot.AddPlotConstraints(openSpaceThreshold);

        ParameterSetup();

        for (int p = 0; p < minNumberOfPlayers; p++)
        {
            GameObject instance = Instantiate<GameObject>(controller.HouseCreator, Vector3.zero + transform.localPosition, Quaternion.identity);
            instance.transform.parent = transform;
            instance.name = $"houseID-{p+1}";
            HouseAgentScript houseAgent = instance.GetComponent<HouseAgentScript>();
            houseAgent.AgentInit((byte)(p+1), (byte)UnityEngine.Random.Range(6, 12), this.plot);
            AgentsList.Add(houseAgent);
            multiAgentGroup.RegisterAgent(AgentsList[p]);
            occupationTarget += houseAgent.houseTargetSize;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ResetScene();
        if (Input.GetKeyDown(KeyCode.Alpha1)) AddAgent();

        AgentsList[currentPlayer - 1].RequestDecision();

        //If time runs over, reset the scene
        if (envMoveSteps >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0 && !reachStableStateOn)
        {
            multiAgentGroup.EndGroupEpisode();
            ResetScene();
        }

        //If more than 1/3 of the maximum environment steps without occupying then terminate
        if (playerTimer >= 0.5f * MaxEnvironmentSteps && !reachStableStateOn)
        {
            //multiAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
            multiAgentGroup.EndGroupEpisode();
        }

        multiAgentGroup.AddGroupReward(-0.5f * (occupationTarget - occupationCount) / MaxEnvironmentSteps);

        if (occupationCount >= occupationTarget && plot.CellTypeCount(CellType.EMPTY) > 15 && occupationCount/(float)availableCellsCount < occupationRateToAddPlayer )
        {
            multiAgentGroup.AddGroupReward(10f);
            AddAgent();
        }

        if (occupationCount / (float)availableCellsCount >= occupationRateLimit && !reachStableStateOn)
        {
            ResetScene();
            multiAgentGroup.EndGroupEpisode();
        }

        if (playerMoves >= playerMovesLimit)
        {
            playerMoves = 0;
            UpdatePlayer();
        }

        playerMoves++;
        envMoveSteps++;
        playerTimer++;
    }
}