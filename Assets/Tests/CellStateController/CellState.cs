using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Custom;


public class CellState : MonoBehaviour
{
    //public Cell2D cell;
    public CellStateController controller;
    int count = 0;

    private void Start()
    {
        controller = GameObject.FindObjectOfType<CellStateController>();
    }

    private void OnMouseDown()
    {
        Debug.Log("mouse down");
        count = count++ % controller.OccupiedCell.Count;
        var go = controller.OccupiedCell[count];
        while (this.transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(0));
        }
        Instantiate(go, this.transform);
        Debug.Log("Mouse down at: " + this.transform.position.x + "," + this.transform.position.x);
    }
}

