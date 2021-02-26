using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForLoopBlock : PanelItem
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        // TODO: not sure if these code will be executed after being destroyed
        if (transform.parent.gameObject.GetComponent<PanelSlot>()) {
            transform.parent.gameObject.GetComponent<PanelSlot>().myPanel.RegisterSkip(gameObject);
        }
    }
}
