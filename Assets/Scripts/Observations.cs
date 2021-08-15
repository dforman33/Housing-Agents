using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observations
{
    /// <summary>
    /// A reference to the agent observing
    /// </summary>
    public int agentID;
    /// <summary>
    /// Cell at the current coordinate
    /// OCCUPIED = 0, EMPTY = 1, OPENAIR (protected) = 2 
    /// </summary>
    public int currentCellType;

    /// <summary>
    /// What is around the agent at any given moment
    /// [0] = EMPTY - binary encoding of positions
    /// [1] = OCCUPIED - binary encoding of positions
    /// [2] = OPEN SPACE - binary encoding of positions
    /// </summary>
    public int[] neighborhoodValues;
    
    /// <summary>
    /// Current size of the agent house
    /// </summary>
    public int selfNeighbourhood;

    /// <summary>
    /// Current size of the agent house
    /// </summary>
    public int horizNeighbourhood;

    /// <summary>
    /// Current size of the agent house
    /// </summary>
    public int currentSize;

    /// <summary>
    /// Type of cell casting a shadow on
    /// OPENGROUND (protected) = -1, OCCUPIED = 0, OTHER = 1
    /// </summary>
    public int shadowOnCellType;
}
