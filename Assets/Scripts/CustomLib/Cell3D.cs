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
            plot = GameObject.FindObjectOfType<Plot3D>();
        }


        public void SetUpCell(int coorX, int coorY, int coorZ, Vector3 vec3Pos)
        {
            coordinate = new Coordinate3D(coorX, coorY, coorZ);
            cellPos = vec3Pos;
            cellScale = GameObject.FindObjectOfType<Plot3D>().scale;
            isFixed = false;

            cellPrefab = Instantiate(GameObject.FindObjectOfType<CellStateController>().EmptyCell, new Vector3(cellPos.x, cellPos.y, cellPos.z), Quaternion.identity);

            UpdatePlayerID(0);
            UpdateCellType();
            //UpdatePrefab();
            Debug.Log("Updated cell position: " + cellPos.x + "," + cellPos.y + "," + cellPos.z);

        }

        public void UpdateCell(byte playerID)
        {
            UpdatePlayerID(playerID);
            UpdateCellType();
            UpdatePrefab();
            UpdateColor();
        }

        public void UpdatePlayerID(byte player)
        {
            playerID = player;
        }

        public void UpdateCellType()
        {
            if (playerID == 0) cellType = CellType.EMPTY;
            else if (playerID == 255) cellType = CellType.OPENSPACE;
            else cellType = CellType.OCCUPIED;
        }

        public void UpdatePrefab()
        {
            Debug.Log("Updated cell position: " + this.cellPos.x + "," + this.cellPos.y + "," + this.cellPos.z);
            //if(cellPrefab != null) Destroy(cellPrefab);
            if (cellType == CellType.EMPTY) { cellPrefab = Instantiate(controller.EmptyCell, cellPos, Quaternion.identity); }
            if (cellType == CellType.OCCUPIED) { cellPrefab = Instantiate(controller.OccupiedCell, cellPos, Quaternion.identity); }
            if (cellType == CellType.OPENSPACE) { cellPrefab = Instantiate(controller.OpenSpaceCell, cellPos, Quaternion.identity); }
        }

        public void UpdateColor()
        {
            cellPrefab.GetComponent<Renderer>().material.color = Tools.GetColor(playerID);
        }

        public void OnMouseDown()
        {
            cellPos = gameObject.transform.localPosition;
            UpdateCell(10);
            if (Input.GetKey(KeyCode.Z)) UpdateCell(10);
            if (Input.GetKey(KeyCode.X)) UpdateCell(255);
        }


    }
}


