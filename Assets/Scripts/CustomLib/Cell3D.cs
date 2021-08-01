using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public class Cell3D
    {
        //the state indicates the player that occupies the cell or if this is public space

        public byte state;
        public int posX;
        public int posY;
        public int posZ;
        public Color cellColor;
        public bool isFixed;

        public Cell3D(int posX, int posY, int posZ)
        {
            state = 0;
            isFixed = false;
            this.posX = posX;
            this.posY = posY;
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


