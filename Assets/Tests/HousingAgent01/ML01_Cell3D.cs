using UnityEngine;

namespace Custom
{
    public class ML01_Cell3D : MonoBehaviour
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
        [HideInInspector] public ML01_Plot3D plot;

        public void SetUpCell(int coorX, int coorY, int coorZ, ML01_Plot3D plot, CellStateController controller, byte playerID)
        {
            coordinate = new Coordinate3D(coorX, coorY, coorZ);
            this.plot = plot;
            this.controller = controller;
            cellScale = plot.scale;
            isFixed = false;
            cellPos = transform.localPosition;
            UpdatePlayerID(playerID);
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
            else if (playerID == 254) cellType = CellType.OPENAIR;
            else if (playerID == 255) cellType = CellType.GROUND;
            else cellType = CellType.OCCUPIED;
        }

        public void UpdatePrefab()
        {

            GameObject oldGo = gameObject;

            if (cellType == CellType.EMPTY)
            {
                SetNewCellInPlot(controller.EmptyCell);
            }
            if (cellType == CellType.OCCUPIED)
            {
                SetNewCellInPlot(controller.OccupiedCell);
            }
            if (cellType == CellType.OPENAIR)
            {
                SetNewCellInPlot(controller.OpenAirCell);
            }
            if (cellType == CellType.GROUND)
            {
                SetNewCellInPlot(controller.GroundCell);
            }

            if (oldGo != null) Destroy(oldGo);
        }

        private void UpdateColor()
        {
            if (cellType == CellType.OCCUPIED)
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z].gameObject.GetComponent<Renderer>().material.color = Tools.GetColor(playerID);
        }

        public void UpdateColor(Color newColor)
        {
            if (cellType == CellType.OCCUPIED || cellType == CellType.GROUND)
                plot.cells[coordinate.X, coordinate.Y, coordinate.Z].gameObject.GetComponent<Renderer>().material.color = newColor;
        }

        private void OnMouseDown()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                UpdateCell(254);
            else
                UpdateCell(10);
        }

        private void SetNewCellInPlot(GameObject prefabType)
        {
            //this is to simplify the routine of updating the prefab
            var newGo = Instantiate(prefabType, cellPos + transform.parent.position, Quaternion.identity);
            newGo.name = "Cell(" + coordinate.X + "," + coordinate.Y + "," + coordinate.Z + ")";
            newGo.transform.parent = plot.transform;
            var newCell = newGo.GetComponent<ML01_Cell3D>();
            newCell.SetUpCell(coordinate.X, coordinate.Y, coordinate.Z, plot, controller, playerID);
            plot.cells[coordinate.X, coordinate.Y, coordinate.Z] = newCell;
        }
    }
}