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
        [HideInInspector] public GameObject cellPrefab;
        [HideInInspector] public bool isFixed;         
        [HideInInspector] public byte playerID;

        [Header("Connections to main plot")]
        [HideInInspector] public CellStateController controller;
        [HideInInspector] public Plot3D plot;

        
        private void Start()
        {
            controller = GameObject.FindObjectOfType<CellStateController>();
            cellScale = GameObject.FindObjectOfType<Plot3D>().scale;
        }


        public void SetUpCell(int coorX, int coorY, int coorZ, Plot3D plot)
        {
            coordinate = new Coordinate3D(coorX, coorY, coorZ);
            this.plot = plot;
            isFixed = false;

            cellPrefab = Instantiate(GameObject.FindObjectOfType<CellStateController>().EmptyCell, new Vector3(cellPos.x, cellPos.y, cellPos.z), Quaternion.identity);

            UpdatePlayerID(0);
            Debug.Log("Cell position: " + cellPos);
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
            else if (playerID > 0 && playerID < 255) cellType = CellType.OPENSPACE;
            else cellType = CellType.OCCUPIED;
        }

        private void UpdatePrefab()
        {
            cellPos = gameObject.transform.localPosition;
            Debug.Log("Updated cell position: " + cellPos);

            GameObject oldGo = cellPrefab;
            
            if (cellType == CellType.EMPTY) { cellPrefab = Instantiate(controller.EmptyCell, cellPrefab.transform.localPosition, Quaternion.identity); }
            if (cellType == CellType.OCCUPIED) { cellPrefab = Instantiate(controller.OccupiedCell, cellPrefab.transform.localPosition, Quaternion.identity); }
            if (cellType == CellType.OPENSPACE) { cellPrefab = Instantiate(controller.OpenSpaceCell, cellPrefab.transform.localPosition, Quaternion.identity); }

            if (oldGo != null) Destroy(oldGo);
        }

        private void UpdateColor()
        {
            this.cellPrefab.GetComponent<Renderer>().material.color = Tools.GetColor(playerID);
        }

        public void OnMouseDown()
        {
            UpdateCell(10);
            if (Input.GetKey(KeyCode.X)) UpdateCell(255);   
        }


    }
}


