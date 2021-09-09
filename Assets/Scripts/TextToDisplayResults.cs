using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

using UnityEngine;
using UnityEngine.UI;

public class TextToDisplayResults : MonoBehaviour
{
    [SerializeField] Text textToDisplay;
    [SerializeField] public string scenarioReference;
    [SerializeField] public string filePath;

    private TrainingEnvironment scene;
    private RecordInfo info;

    //supporting information
    private float occupancyRatio;
    private int numPlayers;
    private float FAR;
    private int scenePackedCells;
    private float avgOccupiedHeight;
    private string timeElapsed;



    // ACTION SUBSCRIBER
    private void Text_OnPlotReset(float occupancyR, int numPlayers, float FAR, int packedOccupiedCount, float avgOccupiedHeight, int timeElapsed)
    {
        info.AddRecording(occupancyR, numPlayers, FAR, packedOccupiedCount, avgOccupiedHeight, timeElapsed);
    }

    private void Awake()
    {
        scene = GetComponent<TrainingEnvironment>();
        info = new RecordInfo();
        info.Initilaise(scenarioReference, filePath);

        //subscribe to actions and events
        scene.OnPlotReset += Text_OnPlotReset;
    }

    private void Update()
    {
        occupancyRatio = (float)(100 * System.Math.Round((double)(scene.occupationCount / (double)scene.availableCellsCount), 2));
        numPlayers = scene.numberOfPlayers;
        FAR = (float) Math.Round((double)(scene.occupationCount / (double)scene.plot.footprintArea), 2);
        scenePackedCells = scene.packedOccupiedCount;
        avgOccupiedHeight = scene.plot.AverageOccupiedHeight();
        timeElapsed = $"{scene.envMoveSteps / 60}:{scene.envMoveSteps % 60}";

        textToDisplay.text = $"BASIC ENVIROMNENT PARAMETERS:"
            + $"\nNumber of iterations: {scene.environmentResets + 1}"
            + $"\nNumber of active players: {scene.activePlayers}"
            + $"\nNumber of inactive players: {scene.inactivePlayers}"
            + $"\nTargeted occupation: {scene.occupationTarget}"
            + $"\nInitial cells available: {scene.availableCellsCount}"
            + $"\nEmpty cells remaining: {scene.emptyCellsCount}"
            + $"\nOccupied cells: {scene.occupationCount}"
            + $"\nOccupied footprint: {scene.occupationFootprint}"
            + $"\nPlot footprint (sq. units): {scene.plot.footprintArea}"
            + $"\nOccupation target: {100 * System.Math.Round(scene.occupationRateLimit, 2)}%"

            + $"\n \nEVALUATION PARAMETERS:"
            + $"\nOccupancy ratio (% of initial available cells): {occupancyRatio}%"
            + $"\nAgents added: {numPlayers}"
            + $"\nOccupied area to footprint ratio (FAR): {FAR}"
            + $"\nCells in packed neighbourhoods: {scenePackedCells}"
            + $"\nAverage ocuppation height: {avgOccupiedHeight}"
            + $"\nTime elapsed: {timeElapsed}"
            ;

    }

    private void OnApplicationQuit()
    {
        info.CloseRecording();
    }

    public class RecordInfo
    {
        public string scenarioReference;
        public string filePath;
        public List<string> results;
        public int iterations;

        public StreamWriter writer;

        public void Initilaise(string scenarioReference, string filePath)
        {
            string filePathAndName = filePath + "\\" + scenarioReference + $"-{DateTime.Now.Day}-{DateTime.Now.Hour}{DateTime.Now.Minute}.csv";
            iterations = 1;
            results = new List<string>();
            writer = new StreamWriter(filePathAndName);
            writer.WriteLine($"Reference: {scenarioReference}, Month: {DateTime.Now.Month}, Day: {DateTime.Now.Day}, Time: {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
            writer.WriteLine($"Iteration,Occupancy_Ratio,Agents,FAR,Packed,Avg_Height,Time");
            writer.Flush();
        }

        public void AddRecording(float occupancyRatio, int agentsAdded, float FAR, int packedCells, float avgOccupiedHt, int timeEllapsed)
        {
            writer.WriteLine(iterations.ToString() + "," + occupancyRatio + "," +  agentsAdded + "," + FAR + "," + packedCells + "," + avgOccupiedHt + "," + timeEllapsed);
            iterations++;
            writer.Flush();
        }

        public void CloseRecording()
        {
            writer.Close();
        }

    }
}
