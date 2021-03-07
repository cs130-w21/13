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

    /// <summary>
    /// Test if the item should be added inside the subpanel, or should be included in the outer panel.
    /// </summary>
    /// <param name="item">GameObject: the item whose position is in question</param>
    /// <returns>bool: if the item should be included in the subpanel</returns>
    bool InTriggerRange(GameObject item);

    void PutItem(GameObject item);
}
