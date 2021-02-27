using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDroppable {
    /// <summary>
    /// the ultimate response to item leaving.
    /// item might do their own thing while being dragged.
    /// </summary>
    /// <param name="item">the item that left this droppable object</param>
    void ItemLeft(GameObject item);

    void ItemCame(GameObject item);

    GameObject GetCurrentItem();

    bool IsOccupied();

    string GetLayer();
}
