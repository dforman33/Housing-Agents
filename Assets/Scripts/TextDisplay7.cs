using UnityEngine;
using UnityEngine.UI;

public class TextDisplay7 : MonoBehaviour
{
    [SerializeField] Text textToDisplay;
    [SerializeField] string messages;
    private Scene07Environment scene;

    private int cellsBuilt;

    private void Awake()
    {
        scene = GetComponent<Scene07Environment>();
    }
    private void Update()
    {
        textToDisplay.text = $"Environment 1 " +
            $"\nNumber of resets: {scene.environmentResets}" +
            $"\nTime elapsed: {scene.envMoveSteps/60} of {scene.MaxEnvironmentSteps/60}" +
            $"\nNumber of active players: {scene.activePlayers}" + 
            $"\nNumber of inactive players: {scene.inactivePlayers}"+
            $"\nOccupied cells: {scene.occupationCount}" +
            $"\nOccupied footprint: {scene.occupationFootprint}" +
            $"\nRatio between ocupied cells and footprint: {System.Math.Round((double)(scene.occupationCount / (double)scene.occupationFootprint), 2)}";
    }
}
