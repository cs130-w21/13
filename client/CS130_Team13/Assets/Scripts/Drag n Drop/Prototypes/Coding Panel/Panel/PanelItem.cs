using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelItem : DraggablePhysics, ICodeInfo
{
    [SerializeField]
    private string myInformation = "";

    [SerializeField]
    private int myCost = 1;
    
    public virtual string GetInformation() {
        return myInformation;
    }

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);

        DragDropManager.instance.currentlyDraggedItem = gameObject;
    }

    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);

        if (DragDropManager.instance.currentlyDraggedItem == gameObject)
            DragDropManager.instance.currentlyDraggedItem = null;

        if (transform.parent == DragDropManager.instance.GetDraggingContainer()) {
            Destroy(gameObject);
        }
    }

    public virtual int GetCost() {
        return myCost;
    }
}
