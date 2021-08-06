using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Custom namespace
using Custom;


namespace Custom 
{
    public class Cell3D : MonoBehaviour
    {
        //the state indicates the player that occupies the cell or if this is public space
        [HideInInspector] public Coordinate3D coordinate;
        [HideInInspector] public Vector3 cellPos;
        [HideInInspector] public float cellScale;

        [HideInInspector] public CellType cellType;
        //[HideInInspector] public GameObject cellPrefab;
        [HideInInspector] public bool isFixed;         
        [HideInInspector] public byte playerID;

        [Header("Connections to main plot")]
        [HideInInspector] public CellStateController controller;
        [HideInInspector] public Plot3D plot;

        public void SetUpCell(int coorX, int coorY, int coorZ, Plot3D plot, CellStateController controller)
        {
            coordinate = new Coordinate3D(coorX, coorY, coorZ);
            this.plot = plot;
            this.controller = controller;

            cellScale = plot.scale;

            isFixed = false;
            cellPos = transform.position;

            //cellPrefab = Instantiate(GameObject.FindObjectOfType<CellStateController>().EmptyCell, cellPos, Quaternion.identity);
            //cellPrefab.transform.parent = transform;

            UpdatePlayerID(0);
            Debug.Log("Cell position: " + cellPos+ " and cell name: " + this.name);
            UpdateCellType();
            
        }

        public void UpdateCell(byte playerID)
        {
            UpdatePlayerID(playerID);
            UpdateCellType();
            UpdatePrefab();
            UpdateColor();
        }

        private void UpdatePlayerID(byte player)
        {
            playerID = player;
        }

        private void UpdateCellType()
        {
            if (playerID == 0) cellType = CellType.EMPTY;
            else if (playerID == 255) cellType = CellType.OPENSPACE;
            else cellType = CellType.OCCUPIED;
        }

        private void UpdatePrefab()
        {

            GameObject oldGo = gameObject;
            
            if (cellType == CellType.EMPTY) { 
                var newGo = Instantiate(controller.EmptyCell, cellPos, Quaternion.identity);
                if (oldGo != null) Destroy(oldGo);
                newGo.transform.parent = plot.transform;
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z] = newGo.GetComponent<Cell3D>();
            }
            if (cellType == CellType.OCCUPIED) {
                var newGo = Instantiate(controller.OccupiedCell, cellPos, Quaternion.identity);
                if (oldGo != null) Destroy(oldGo);
                newGo.transform.parent = plot.transform;
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z] = newGo.GetComponent<Cell3D>();
            }
            if (cellType == CellType.OPENSPACE) { 
                var newGo = Instantiate(controller.OpenSpaceCell, cellPos, Quaternion.identity);
                if (oldGo != null) Destroy(oldGo);
                newGo.transform.parent = plot.transform;
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z] = newGo.GetComponent<Cell3D>();
            }
        }

        private void UpdateColor()
        {
            if (cellType == CellType.OCCUPIED)
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z].gameObject.GetComponent<Renderer>().material.color = Tools.GetColor(playerID);
        }

        public void OnMouseDown()
        {
            if (Input.GetKey(KeyCode.LeftShift)) 
                UpdateCell(255);
            else
                UpdateCell(10);
        }


    }
}


