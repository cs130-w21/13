using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ForLoopBlock : PanelItem, ISubPanel
{
    [SerializeField]
    private GameObject myPanel = null;

    [SerializeField]
    private GameObject loopCounter = null;

    /// <summary>
    /// simulate a guard probe event if dragged item is not for loop
    /// </summary>
    public void IsOccupied() {
        if (DragDropManager.instance.currentlyDraggedItem.GetComponent<ForLoopBlock>())
            return;
        else
            myPanel.GetComponent<CodingPanel>().ReportGuardProbe();
    }

    public void ItemCame(GameObject newItem)
    {
        if (DragDropManager.instance.currentlyDraggedItem.GetComponent<ForLoopBlock>())
            return;
        else
            myPanel.GetComponent<CodingPanel>().PutItem(newItem);
        
        Debug.Log(GetInformation());
    }

    public override string GetInformation() {
        string result = "l" + (loopCounter.GetComponent<Dropdown>().value + 1).ToString() + "{";
        
        // get information from children blocks
        result += myPanel.GetComponent<CodingPanel>().GetInformation();
        result += "}";

        return result;
    }

    public override int GetCost() {
        int result = 0;

        result += myPanel.GetComponent<CodingPanel>().GetCost();

        return result;
    }
}
