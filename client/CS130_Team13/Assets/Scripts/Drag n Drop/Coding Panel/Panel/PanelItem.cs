using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// attach to object to make it a PanelItem, derived class of DraggablePhysics;
/// panel item adds to the DraggablePhysics behavior by:
/// 1) destroys self if it can't snap into a droppable on mouse release,
/// 2) registers itself with DragDropManager to allow global access from other objects;
/// in addition, it implements ICodeInfo interface
/// </summary>
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
