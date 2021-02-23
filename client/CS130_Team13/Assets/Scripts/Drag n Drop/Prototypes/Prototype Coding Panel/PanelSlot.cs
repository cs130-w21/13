using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PanelSlot : DroppablePhysics, ICodeInfo
{
    public CodingPanel myPanel;

    private string myInfo = "";

    [SerializeField]
    private bool foolSwtich = false;

    public override void ItemCame(GameObject item) {
        base.ItemCame(item);

        myInfo = item.GetComponent<ICodeInfo>().GetInformation();
    }

    public override void ItemLeft(GameObject item) {
        base.ItemLeft(item);

        myPanel.RemoveSlot(gameObject);
    }

    public string GetInformation() {
        return myInfo;
    }
}
