using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Custom library
using Custom;

public class CellBehaviour : MonoBehaviour
{
    //replaces the previous cellState test
    [HideInInspector] public Cell3D cell;
    [HideInInspector] public Plot3D plot;
    [HideInInspector] public CellStateController controller;
    [HideInInspector] public GameObject cellGameObject;

    private void Start()
    {
        plot = GameObject.FindObjectOfType<Plot3D>();
        controller = GameObject.FindObjectOfType<CellStateController>();
        
        //UpdateCell(State.EMPTY, 0);
    }
    public void SetupCell(int x, int y, int z)
    {
        cell = new Cell3D(x, y, z);
        cellGameObject = controller.EmptyCell;
    }
    public void UpdateCell(State state, byte playerID)
    {
        Transform trnsf = this.transform;


        if (state == State.EMPTY)
        {
            cell.updateCell(playerID);
            cellGameObject = controller.EmptyCell;
            Color col = Color.blue;
            col.a = 0.25f;
            cellGameObject.GetComponent<Renderer>().material.color = col;
        }

        //player ID will determine color
        if (state == State.OCCUPIED)
        {
            cell.updateCell(playerID);
            cellGameObject = controller.OccupiedCell;
            Color col = Color.blue;
            col.a = 0.75f;
            cellGameObject.GetComponent<Renderer>().material.color = col;

        }
        if (state == State.OPENSPACE)
        {
            cell.updateCell(playerID);
            cellGameObject = controller.EmptyCell;
            Color col = Color.white;
            col.a = 0.1f;
            cellGameObject.GetComponent<Renderer>().material.color = col;
        }
    }

}
