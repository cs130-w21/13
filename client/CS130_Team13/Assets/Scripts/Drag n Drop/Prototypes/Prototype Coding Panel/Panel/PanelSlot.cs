using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    }

    public override void ItemLeft(GameObject item) {
        base.ItemLeft(item);

        myPanel.RemoveSlot(gameObject);
        myInfo = "";
        myCost = 0;
    }

    public string GetInformation() {
        return myInfo;
    }

    public int GetCost() {
        return myCost;
    }
}
