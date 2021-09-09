using UnityEngine;
using UnityEngine.UI;

public class TextToDisplayResults : MonoBehaviour
{
    [SerializeField] Text textToDisplay;
    [SerializeField] string messages;
    private TrainingEnvironment scene;

    private int cellsBuilt;

    private void Awake()
    {
        scene = GetComponent<TrainingEnvironment>();
    }
    private void Update()
    {
        textToDisplay.text = $"BASIC ENVIROMNENT PARAMETERS:"
            + $"\nNumber of resets: {scene.environmentResets}"
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
            + $"\nOccupancy ratio (% of initial available cells): {100 * System.Math.Round((double)(scene.occupationCount / (double)scene.availableCellsCount), 2)}%"
            + $"\nAgents added: {scene.numberOfPlayers}"
            + $"\nOccupied area to footprint ratio (FAR): {System.Math.Round((double)(scene.occupationCount / (double)scene.plot.footprintArea), 2)}"
            + $"\nCells in packed neighbourhoods: {scene.packedOccupiedCount}"
            + $"\nTime elapsed: {scene.envMoveSteps / 60}"
            ;
    }
}
