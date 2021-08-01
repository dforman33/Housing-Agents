using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{    public class Cell2D
    {
        //the state indicates the player that occupies the cell or if this is public space

        public byte state;
        public int posX;
        public int posZ;
        public Color cellColor;
        public bool isFixed;

        public Cell2D(int posX, int posZ)
        {
            state = 0;
            isFixed = false;
            this.posX = posX;
            this.posZ = posZ;
        }

        public void updateCell(byte stateID)
        {
            UpdateState(stateID);
            UpdateColor();
        }

        void UpdateState(byte stateID)
        {
            state = stateID;
        }

        void UpdateColor()
        {
            cellColor = Tools.GetColor(state);
        }
    }
}


