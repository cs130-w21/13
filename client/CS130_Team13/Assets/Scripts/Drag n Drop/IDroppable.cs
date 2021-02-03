using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDroppable
{
    void ItemLeft(GameObject item);
    void ItemCame(GameObject item);
    GameObject GetCurrentItem();
}
