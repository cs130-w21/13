﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // public interface ////////////////////////////////////////////////////////////////////
    public string GetInformation() {
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


    public void RemoveSlot(GameObject deprecatedSlot) {
        mySlots.Remove(deprecatedSlot);
        Destroy(deprecatedSlot);
    }


    // message with guard (handling empty) //////////////////////////////////////////////////
    public void ReportGuardProbe() {
        guardProbed = true;
    }


    // system messages //////////////////////////////////////////////////////////////////////
    public void LateUpdate() {
        if (hoveringSlot && hoveringSlot.GetComponent<IDroppable>().IsOccupied()) {
            hoveringSlot = null;
        }

        // if there's an item hoverring above this panel
        if (guardProbed) {
            // if there's an item currently being dragged
            if (DragDropManager.instance.currentlyDraggedItem) {
                // in case of empty
                if (mySlots.Count == 0) {
                    hoveringSlot = FormatNewSlot(0);
                }
                else {
                    bool touched = false;
                    
                    // position check
                    foreach (GameObject slot in mySlots) {
                        Vector2 slotBottom = JohnnyUITools.GetCanvasCoord(slot);
                        Vector2 slotTop = JohnnyUITools.GetCanvasCoord(slot) + slot.GetComponent<RectTransform>().sizeDelta;
                        Vector2 itemCenter = JohnnyUITools.GetCenterCanvasCoord(DragDropManager.instance.currentlyDraggedItem);

                        // move or create hovering slot here
                        if (slotBottom.y <= itemCenter.y && slotTop.y >= itemCenter.y) {
                            touched = true;

                            if (slot == hoveringSlot) break;

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
                    }

                    if (!touched) {
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
}
