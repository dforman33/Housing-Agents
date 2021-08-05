using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Custom;


public class CellState : MonoBehaviour
{
    //public Cell2D cell;
    public Cell2D cell;
    //private static int count;
    private Board _board;

    [HideInInspector] public CellStateController controller;

    private void Start()
    {
        //count = 0;
        //controller = GetComponent<CellStateController>();
        controller = GameObject.FindObjectOfType<CellStateController>();
        _board = GameObject.FindObjectOfType<Board>();
    }

    private void OnMouseDown()
    {
        //count = count++ % controller.OccupiedCell.Count;
        //var go = controller.OccupiedCell[count];
        //count++;

        //Transform transf = this.transform;
        var occupied = controller.OccupiedCell;
        Destroy(gameObject);

        //int loops = 0;

        //while (this.transform.childCount > 0 && loops < 10)
        //{
        //    loops++;
        //    Destroy(this.transform.GetChild(0).gameObject);
        //}

        var child = Instantiate(occupied, transform.position, transform.rotation);

        if (Input.GetKey(KeyCode.C))
        {
            child.GetComponent<Renderer>().material.color = Color.blue;
        }

        else if (Input.GetKey(KeyCode.LeftShift))
        {
            Destroy(child.gameObject);
            Instantiate(controller.OpenSpaceCell, transform.position, transform.rotation);
        }
    }
}

