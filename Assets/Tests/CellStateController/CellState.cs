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
        //controller = GetComponent<CellStateController>();
        controller = GameObject.FindObjectOfType<CellStateController>();
    }

    private void OnMouseDown()
    {
        //count = count++ % controller.OccupiedCell.Count;
        Transform transf = this.transform;
        var go = controller.OpenSpaceCell;
        int first = this.GetInstanceID();
        Debug.Log("Object ID found " + first);

        //if(this != null) Destroy(this.transform.GetChild(0));
        Destroy(this.transform.GetChild(0));
        //int countLoop = 0;
        //while (this.transform.childCount > 0 || countLoop < 10)
        //{
        //    countLoop++;
        //    Destroy(this.transform.GetChild(0));
        //}
        Instantiate(go, transf);
        Debug.Log("Mouse down at: " + this.transform.position.x + "," + this.transform.position.x);
    }
}

