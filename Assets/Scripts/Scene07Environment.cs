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

public class Scene07Environment : MonoBehaviour
{
    [HideInInspector] private Plot3D plot;
    [HideInInspector] public int[,] heightMap;
    [HideInInspector] public CellStateController controller;
   
    [HideInInspector] public byte developmentTargetSize;
    [SerializeField] private int numberOfPlayers = 3;
    [HideInInspector] private int TargetOccupation = 0;
    [HideInInspector] private int occupationCount = 0;

    //Environment parameters
    public int MaxEnvironmentSteps = 7200;
    [HideInInspector] int envMoveSteps = 0;
    public int maxMoveSteps = 30;
    [HideInInspector] int moveSteps = 0;
    [HideInInspector] private int m_ResetTimer;


    [HideInInspector] public SimpleMultiAgentGroup multiAgentGroup;
    private List<HouseAgentScript> AgentsList = new List<HouseAgentScript>();
    private int currentPlayer;

    // ACTION SUBSCRIBER
    
    private void Plot_OnOccupyCell(int playerID)
    {
        UpdatePlayer();
        UpdateOccupationCount();
    }

    // METHODS

    /// <summary>
    /// Moves to the next player within the list of players.
    /// </summary>
    private void UpdatePlayer()
    {
        //AgentsList[currentPlayer].isActive = false;
        currentPlayer++;
        if (currentPlayer > numberOfPlayers) currentPlayer = 1;
        //AgentsList[currentPlayer].isActive = true;
    }

    /// <summary>
    /// Updates the total number of occupations considering all agents and players.
    /// </summary>
    private void UpdateOccupationCount()
    {
        int count = 0;
        foreach (var agent in AgentsList)
        {
            count += agent.houseCurrentSize;
        }
        occupationCount = count;
    }

    /// <summary>
    /// Called when the whole scene needs to be cleaned and values reset to the initial state.
    /// </summary>
    public void ResetScene()
    {
        plot.CleanBoard();
        plot.AddPlotConstraints();
        heightMap = plot.heightMap;
        //Reset Agents
        foreach (var agent in AgentsList) { agent.AgentRestartEpisode(heightMap); }
        envMoveSteps = 0;
        moveSteps = 0;
        occupationCount = 0;
        currentPlayer = 1;
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
        plot.AddPlotConstraints();
        heightMap = plot.heightMap;
        currentPlayer = 1;

        for (int p = 0; p < numberOfPlayers; p++)
        {
            GameObject instance = Instantiate<GameObject>(controller.HouseCreator, Vector3.zero + transform.localPosition, Quaternion.identity);
            instance.transform.parent = transform;
            instance.name = $"houseID-{p+1}";
            HouseAgentScript houseAgent = instance.GetComponent<HouseAgentScript>();
            houseAgent.AgentInit((byte)(p+1), (byte)(10 + 2 * p), this.plot);
            AgentsList.Add(houseAgent);
            multiAgentGroup.RegisterAgent(AgentsList[p]);
            TargetOccupation += houseAgent.houseTargetSize;
        }
    }

    void Update()
    {
        if (envMoveSteps >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            multiAgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }

        if (TargetOccupation == occupationCount)
        {
            multiAgentGroup.AddGroupReward(10f * numberOfPlayers);
            ResetScene();
            multiAgentGroup.EndGroupEpisode();
        }

        //penalty on time to accomplish aggregation
        multiAgentGroup.AddGroupReward(-0.1f * numberOfPlayers / MaxEnvironmentSteps);

        AgentsList[currentPlayer - 1].RequestDecision();

        if (moveSteps == maxMoveSteps) { UpdatePlayer(); }

        moveSteps++;
        envMoveSteps++;
        Debug.Log($"Number of steps: {envMoveSteps}");
        if(moveSteps >= maxMoveSteps)
        {
            moveSteps = 0;
            UpdatePlayer();
        }
    }



}