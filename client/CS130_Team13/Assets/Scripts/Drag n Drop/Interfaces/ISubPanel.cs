using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to tell parent panel this item contains another coding panel, 
/// used to let items decide where to put their panel separately from the parent structure
/// </summary>
public interface ISubPanel
{
    void IsOccupied();
    void ItemCame(GameObject item);
}
