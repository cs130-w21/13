using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICodeInfo
{
    /// <summary>
    /// this function should return a draggable item (code block)'s information.
    /// if-statements and for-loops can be implemented with a healthy composite pattern
    /// </summary>
    /// <returns>string: the information extracted from this block</returns>
    string GetInformation();

    int GetCost();
}
