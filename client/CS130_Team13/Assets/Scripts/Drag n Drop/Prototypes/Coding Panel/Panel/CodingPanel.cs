using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// there needs to be a 
/// </summary>
public class CodingPanel : MonoBehaviour, ICodeInfo {
    [SerializeField]
    private PanelGuard myGuard;

    [SerializeField]
    private GameObject mySlotInstance;

    // guard message
    private bool guardProbed = false;

    // need to make sure its order corresponds to positional order
    [SerializeField]
    private List<GameObject> mySlots = new List<GameObject>();

    // there should only be one "hovering" empty slot instantiated in this
    private GameObject hoveringSlot = null;

    // deciding the max amount of items
    [SerializeField]
    private int maxCost = 10;

    // remember which items are in the panel
    private HashSet<GameObject> myItems = new HashSet<GameObject>();

    private HashSet<GameObject> skipCheckItems = new HashSet<GameObject>();

    // public interface ////////////////////////////////////////////////////////////////////
    public virtual string GetInformation() {
        string newInformation = "";

        foreach (GameObject slot in mySlots) {
            if (slot.GetComponent<PanelSlot>().IsOccupied()) {
                if (slot.GetComponent<ICodeInfo>().GetInformation() != "") {
                    newInformation += slot.GetComponent<ICodeInfo>().GetInformation() + " ";
                }
            }
        }

        return newInformation;
    }

    public virtual int GetCost() {
        int cost = 0;

        foreach (GameObject slot in mySlots) {
            cost += slot.GetComponent<ICodeInfo>().GetCost();
        }

        return cost;
    }

    public virtual void PutItem(GameObject newItem) {
        // force item into hovering slot
        if (hoveringSlot) {
            // put item in slot
            newItem.GetComponent<IDraggable>().ForceInto(hoveringSlot);
            // update items
            myItems.Add(newItem);
        }
        // TODO: handle the situation where no slot is hovering
        else {
            throw new System.Exception("it happened! the object is releaed while no slot is available! tell johnny!");
        }
    }

    public virtual void RegisterSkip(GameObject newItem) {
        skipCheckItems.Add(newItem);
    }


    // message with guard (handling empty) //////////////////////////////////////////////////
    /// <summary>
    /// register a guard being probed event, and tell the panel guard if panel can take the new object
    /// </summary>
    /// <returns>bool: panel has enough space (true = y)</returns>
    public virtual bool ReportGuardProbe() {
        guardProbed = true;
        
        return PanelHasEnoughSpace() /*&& !myItems.Contains(DragDropManager.instance.currentlyDraggedItem)*/;
    }


    // system messages //////////////////////////////////////////////////////////////////////
    public virtual void LateUpdate() {
        if (hoveringSlot && hoveringSlot.GetComponent<IDroppable>().IsOccupied()) {
            hoveringSlot = null;
        }

        // if there's an item hoverring above this panel
        if (guardProbed /*&& !myItems.Contains(DragDropManager.instance.currentlyDraggedItem)*/) {
            // skip if panel is full already
            if (!PanelHasEnoughSpace()) {
                // TODO: fill in the alarming behavior here
                Debug.Log("panel full!");
                guardProbed = false;
                return;
            }

            // if there's an item currently being dragged
            if (DragDropManager.instance.currentlyDraggedItem) {
                // in case of empty
                if (mySlots.Count == 0) {
                    hoveringSlot = FormatNewSlot(0);
                }
                else {
                    bool matched = false;
                    bool above = false;
                    
                    // position check
                    foreach (GameObject slot in mySlots) {
                        GameObject slotItem = slot.GetComponent<IDroppable>().GetCurrentItem();

                        Vector2 slotBottom = JohnnyUITools.GetCanvasCoord(slot);
                        Vector2 slotTop = JohnnyUITools.GetCanvasCoord(slot) + slot.GetComponent<RectTransform>().sizeDelta;
                        Vector2 itemBottom = JohnnyUITools.GetCanvasCoord(DragDropManager.instance.currentlyDraggedItem);
                        Vector2 itemCenter = JohnnyUITools.GetCenterCanvasCoord(DragDropManager.instance.currentlyDraggedItem);

                        // move or create hovering slot here
                        if (slotBottom.y <= itemCenter.y && slotTop.y >= itemCenter.y) {
                            matched = true;
                            
                            if (slot == hoveringSlot) break;
                            else if (skipCheckItems.Contains(slotItem)) {
                                // clear hovering slot
                                if (hoveringSlot) {
                                    RemoveSlot(hoveringSlot);
                                    hoveringSlot = null;    
                                }
                                break;
                            }

                            // create new hover slot
                            if (!hoveringSlot) {
                                hoveringSlot = FormatNewSlot(mySlots.IndexOf(slot));
                            }
                            // rearrange slots
                            else {
                                int newIndex;
                                // above current slot
                                if (itemCenter.y - slotBottom.y < slotTop.y - itemCenter.y) {
                                    newIndex = (mySlots.IndexOf(hoveringSlot) < mySlots.IndexOf(slot)) ?
                                        mySlots.IndexOf(slot) : mySlots.IndexOf(slot) + 1;
                                }
                                // below current slot
                                else {
                                    newIndex = (mySlots.IndexOf(hoveringSlot) > mySlots.IndexOf(slot)) ?
                                        mySlots.IndexOf(slot) : mySlots.IndexOf(slot) - 1;
                                }

                                ReorderSlot(newIndex, hoveringSlot);
                            }

                            break;
                        }
                        else if (slotBottom.y < itemBottom.y) {
                            above = true;
                        }
                    }

                    if (!matched && above) {
                        if (skipCheckItems.Contains(mySlots[0].GetComponent<IDroppable>().GetCurrentItem())) {
                            if (hoveringSlot) {
                                    RemoveSlot(hoveringSlot);
                                    hoveringSlot = null;    
                            }
                        }
                        else if (hoveringSlot) {
                            ReorderSlot(0, hoveringSlot);
                        }
                        else {
                            hoveringSlot = FormatNewSlot(0);
                        }
                    }
                    else if (!matched) {                        
                        // move hover slot to bottom
                        if (hoveringSlot) {
                            ReorderSlot(mySlots.Count - 1, hoveringSlot);
                        }
                        else {
                            hoveringSlot = FormatNewSlot(mySlots.Count);
                        }
                    }
                }
            }
        }

        else if (hoveringSlot) {
            RemoveSlot(hoveringSlot);
            hoveringSlot = null;
        }

        guardProbed = false;
    }


    // utils ////////////////////////////////////////////////////////////////////////////////
    private void ReorderSlot(int index, GameObject slot) {
        slot.transform.SetSiblingIndex(index);
        mySlots.Remove(slot);
        mySlots.Insert(index, slot);
    }
        
    private GameObject FormatNewSlot(int index) {
        GameObject newSlot = Instantiate(mySlotInstance, transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.GetComponent<PanelSlot>().myPanel = this;

        // make sure every slot 
        if (DragDropManager.instance.currentlyDraggedItem != null) {
            newSlot.GetComponent<RectTransform>().sizeDelta =
                DragDropManager.instance.currentlyDraggedItem.GetComponent<RectTransform>().sizeDelta;
        }

        newSlot.transform.SetSiblingIndex(index);
        mySlots.Insert(index, newSlot);

        return newSlot;
    }

    public virtual void RemoveSlot(GameObject deprecatedSlot) {
        GameObject depItem = deprecatedSlot.GetComponent<IDroppable>().GetCurrentItem();
        
        // update slots
        mySlots.Remove(deprecatedSlot);
        // update items
        myItems.Remove(depItem);
        // update skip item
        if (skipCheckItems.Contains(depItem)) {
            skipCheckItems.Remove(depItem);
        }
        Destroy(deprecatedSlot);
    }

    private bool PanelHasEnoughSpace() {
        // access the dragged item for 
        int inpendingCost = DragDropManager.instance.currentlyDraggedItem.GetComponent<ICodeInfo>().GetCost();
        return (GetCost() + inpendingCost <= maxCost);
    }
}
