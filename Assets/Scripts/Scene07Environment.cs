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
/// This environment class controls the agents and the plot class. 
/// It assumes a fixed number of agents and tries to optimise the aggregation for them.
/// </summary>

public class Scene07Environment : MonoBehaviour
{
    [HideInInspector] private Plot3D plot;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public CellStateController controller;
    [HideInInspector] public byte developmentTargetSize;
    [HideInInspector] private int occupationTarget;
    [HideInInspector] public int occupationCount = 0;
    [HideInInspector] public int occupationFootprint = 0;
    [HideInInspector] public int environmentResets = 0;
    [HideInInspector] public int activePlayers = 0;
    [HideInInspector] public int inactivePlayers = 0;

    [SerializeField] private int numberOfPlayers = 4;
    [SerializeField] public int MaxEnvironmentSteps = 3600;
    [HideInInspector] public int envMoveSteps = 0;
    [SerializeField] private int playerTimer = 180;
    [SerializeField] private int playerMovesLimit = 15;
    [SerializeField] private bool reachStableStateOn = false;

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
    }

    /// <summary>
    /// Called when the whole scene needs to be cleaned and values reset to the initial state.
    /// </summary>
    public void ResetScene()
    {
        int newWidth = UnityEngine.Random.Range(8, 20);
        int newHeight = UnityEngine.Random.Range(8, 20);
        int newDepth = UnityEngine.Random.Range(8, 20);
        // This should provide an open space threshold ranging between 50 and 80, meaning 50 and 20% of open space
        int openSpaceThreshold = 50 + 10 * UnityEngine.Random.Range(0, 4);

        //plot.ResetBoard(newWidth, newHeight, newDepth, openSpaceThreshold);

        //Read new heightmap
        heightMap = plot.heightMap;
        //Reset Agents
        foreach (var agent in AgentsList) { agent.AgentRestartEpisode(heightMap, plot); }

        envMoveSteps = 0;
        playerMoves = 0;
        playerTimer = 0;
        occupationCount = 0;
        currentPlayer = 1;
        environmentResets++;
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
        houseAgent.AgentInit((byte)numberOfPlayers, (byte)UnityEngine.Random.Range(6,12), this.plot);
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
        //Multi-agent config
        multiAgentGroup = new SimpleMultiAgentGroup();
        AgentsList = new List<HouseAgentScript>();
    }

    private void Start()
    {
        //Add constraints
        int openSpaceThreshold = 75;
        plot.AddPlotConstraints(openSpaceThreshold);
        heightMap = plot.heightMap;

        envMoveSteps = 0;
        playerMoves = 0;
        playerTimer = 0;
        occupationCount = 0;
        currentPlayer = 1;
        environmentResets = 0;

        for (int p = 0; p < numberOfPlayers; p++)
        {
            GameObject instance = Instantiate<GameObject>(controller.HouseCreator, Vector3.zero + transform.localPosition, Quaternion.identity);
            instance.transform.parent = transform;
            instance.name = $"houseID-{p+1}";
            HouseAgentScript houseAgent = instance.GetComponent<HouseAgentScript>();
            houseAgent.AgentInit((byte)(p+1), (byte)(10 + 2 * p), this.plot);
            AgentsList.Add(houseAgent);
            multiAgentGroup.RegisterAgent(AgentsList[p]);
            occupationTarget += houseAgent.houseTargetSize;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ResetScene();

        AgentsList[currentPlayer - 1].RequestDecision();
        
        //If time runs over, reset the scene
        if (envMoveSteps >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0 && !reachStableStateOn)
        {
            multiAgentGroup.EndGroupEpisode();
            ResetScene();
        }

        //If more than 1/3 of the maximum environment steps without occupying then terminate
        if (playerTimer >= 0.3f * MaxEnvironmentSteps && !reachStableStateOn)
        {
            //multiAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
            multiAgentGroup.EndGroupEpisode();
        }

        multiAgentGroup.AddGroupReward(-10f * (occupationTarget - occupationCount) / MaxEnvironmentSteps);

        if (occupationCount >= occupationTarget)
        {
            multiAgentGroup.AddGroupReward(10f);
            ResetScene();
            multiAgentGroup.EndGroupEpisode();
        }

        if (plot.CellTypeCount(CellType.EMPTY) < 0.1 * occupationCount && !reachStableStateOn)
        {
            multiAgentGroup.AddGroupReward(10f);
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