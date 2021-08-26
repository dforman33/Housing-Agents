using UnityEngine;
using UnityEngine.UI;

public class TextDisplay8 : MonoBehaviour
{
    [SerializeField] Text textToDisplay;
    [SerializeField] string messages;
    private Scene08Environment scene;

    private int cellsBuilt;

    private void Awake()
    {
        scene = GetComponent<Scene08Environment>();
    }
    private void Update()
    {
        textToDisplay.text = $"Environment 1 " +
            $"\nNumber of resets: {scene.environmentResets}" +
            $"\nTime elapsed: {scene.envMoveSteps/60} of {scene.MaxEnvironmentSteps/60}" +
            $"\nTotal number of players: {scene.numberOfPlayers}" +
            $"\nNumber of active players: {scene.activePlayers}" + 
            $"\nNumber of inactive players: {scene.inactivePlayers}"+
            $"\nTargeted occupation: {scene.occupationTarget}" +
            $"\nInitial cells available: {scene.availableCellsCount}" +
            $"\nEmpty cells remaining: {scene.emptyCellsCount}" +
            $"\nPercentage of occupation: {100 * System.Math.Round((double)(scene.occupationCount / (double)scene.availableCellsCount), 2)}%" +
            $"\nOccupation rate limit allowed: {scene.occupationRateLimit}" +
            $"\nOccupied cells: {scene.occupationCount}" +
            $"\nOccupied footprint: {scene.occupationFootprint}" +
            $"\nRatio between ocupied cells and footprint: {System.Math.Round((double)(scene.occupationCount / (double)scene.occupationFootprint), 2)}";
    }
}
