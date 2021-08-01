using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    [SerializeField] Text textToDisplay;
    [SerializeField] string [] messages;

    private int cellsBuilt;

    private void Start()
    {
        cellsBuilt = 0;

        messages = new string[] {
        "Click on cells to build or press space to protect spaces",
        "Cell added", 
        "Protected space added", 
        "Cells built: " + cellsBuilt.ToString()
        };

        textToDisplay.text = messages[0];
    }
}
