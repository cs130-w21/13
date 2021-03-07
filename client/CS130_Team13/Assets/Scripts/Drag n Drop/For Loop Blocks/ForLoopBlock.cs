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

    [SerializeField]
    private GameObject myHeader = null;

    /// <summary>
    /// simulate a guard probe event if dragged item is not for loop
    /// </summary>
    public virtual void IsOccupied() {
        if (DragDropManager.instance.currentlyDraggedItem.GetComponent<ForLoopBlock>())
            return;
        else
            myPanel.GetComponent<CodingPanel>().ReportGuardProbe();
    }

    public virtual bool InTriggerRange(GameObject item) {
        // get item position (center line)
        Vector2 center = JohnnyUITools.GetCenterCanvasCoord(item);

        // compare item position with header
        Vector2 headerCenter = JohnnyUITools.GetCenterCanvasCoord(myHeader);
        Vector2 headerTop = JohnnyUITools.GetTopCanvasCoord(myHeader);

        if (center.y > headerCenter.y && center.y <= headerTop.y) {
            return false;
        }
        else {
            return true;
        }
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
