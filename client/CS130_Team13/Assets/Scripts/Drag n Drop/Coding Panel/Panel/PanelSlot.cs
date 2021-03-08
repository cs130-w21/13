﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach to object to make it slot that works with CodingPanel;
/// it implements ICodeInfo overrides DroppablePhysics and adds to it:
/// 1) update its own info and cost on item leaving and coming;
/// </summary>
public class PanelSlot : DroppablePhysics, ICodeInfo
{
    public CodingPanel myPanel;

    private string myInfo = "";
    private int myCost = 0;

    //[SerializeField]
    //private bool foolSwtich = false;

    public override void ItemCame(GameObject item) {
        base.ItemCame(item);

        myInfo = item.GetComponent<ICodeInfo>().GetInformation();
        myCost = item.GetComponent<ICodeInfo>().GetCost();

        // open size listener
        gameObject.GetComponent<ContainerResizer>().enabled = true;
    }

    public override void ItemLeft(GameObject item) {
        // close size listener
        gameObject.GetComponent<ContainerResizer>().enabled = false;

        base.ItemLeft(item);

        myPanel.RemoveSlot(gameObject);
        myInfo = "";
        myCost = 0;
    }

    public string GetInformation() {
        return (GetCurrentItem() != null) ? GetCurrentItem().GetComponent<ICodeInfo>().GetInformation() : "";
    }

    public int GetCost() {
        return (GetCurrentItem() != null) ? GetCurrentItem().GetComponent<ICodeInfo>().GetCost() : 0;
    }
}
