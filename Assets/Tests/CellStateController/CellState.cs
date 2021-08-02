using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Custom;


public class CellState : MonoBehaviour
{
    //public Cell2D cell;
    public Cell2D cell;
    public CellStateController controller;
    public static int count;
    public Board _board;

    private void Start()
    {
        count = 0;
        //controller = GetComponent<CellStateController>();
        controller = GameObject.FindObjectOfType<CellStateController>();
        _board = GameObject.FindObjectOfType<Board>();
    }

    private void OnMouseDown()
    {
        count = count++ % controller.OccupiedCell.Count;
        Transform transf = this.transform;
        var go = controller.OccupiedCell[count];

        //Destroy(this.transform.GetChild(0));
        int loops = 0;
        while (this.transform.childCount > 0 && loops < 10)
        {
            loops++;
            Destroy(this.transform.GetChild(0));
        }

        transform.localScale = new Vector3(.25f, .25f, .25f);
        Instantiate(go, transf);
    }
}

