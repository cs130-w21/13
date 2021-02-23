using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodingPanel : MonoBehaviour, ICodeInfo {
    [SerializeField]
    private ProtoPanelGuard myGuard;

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
            if (slot.GetComponent<ProtoPanelSlot>().IsOccupied()) {
                if (slot.GetComponent<ICodeInfo>().GetInformation() != "") {
                    newInformation += slot.GetComponent<ICodeInfo>().GetInformation() + " ";
                }
            }
        }

        return newInformation;
    }


    // messages with slots /////////////////////////////////////////////////////////////////
    //public void OccupiedProbe(GameObject probedObject) {
    //    occupiedSlotCondition.Add(probedObject);
    //}

    //public void UnoccupiedProbe(GameObject probedObject) {
    //    unoccupiedSlotCondition.Add(probedObject);
    //}

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
                    // TODO: position check
                    foreach (GameObject slot in mySlots) {
                        if (slot == hoveringSlot) continue;

                        Vector2 slotBottom = JohnnyUITools.GetCanvasCoord(slot);
                        Vector2 slotTop = JohnnyUITools.GetCanvasCoord(slot) + slot.GetComponent<RectTransform>().sizeDelta;
                        Vector2 itemCenter = JohnnyUITools.GetCenterCanvasCoord(DragDropManager.instance.currentlyDraggedItem);

                        //// if a slot hovers above, simulate the situation of it's not there
                        //if (hoveringSlot && mySlots.IndexOf(hoveringSlot) < mySlots.IndexOf(slot)) {
                        //    slotTop += hoveringSlot.GetComponent<RectTransform>().sizeDelta;
                        //    slotBottom += hoveringSlot.GetComponent<RectTransform>().sizeDelta;
                        //}

                        // move or create hovering slot here
                        if (slotBottom.y <= itemCenter.y && slotTop.y >= itemCenter.y) {
                            //if (hoveringSlot) {
                            //    int newIndex = mySlots.IndexOf(slot);
                            //    hoveringSlot.transform.SetSiblingIndex(newIndex);
                            //    mySlots.Remove(hoveringSlot);
                            //    mySlots.Insert(newIndex, hoveringSlot);
                            //}
                            //else {
                            //    // instantiate
                            //    hoveringSlot = FormatNewSlot(mySlots.IndexOf(slot));
                            //}

                            int newIndex = -1;
                            
                            // create new hover slot
                            if (!hoveringSlot) {
                                hoveringSlot = FormatNewSlot(mySlots.IndexOf(slot));
                                break;
                            }

                            // rearrange hover slot
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

                            hoveringSlot.transform.SetSiblingIndex(newIndex);
                            mySlots.Remove(hoveringSlot);
                            mySlots.Insert(newIndex, hoveringSlot);

                            break;
                        }
                    }
                }
            }
        }

        if (!guardProbed && hoveringSlot) {
            RemoveSlot(hoveringSlot);
            hoveringSlot = null;
        }

        guardProbed = false;
    }

    //public void LateUpdate() {

    //    if (guardProbed && mySlots.Count == 0) {
    //        // instantiate the first slot
    //        FormatNewSlot(0);
    //        guardProbed = false;
    //        return;
    //    }
    //    else if (mySlots.Count == 1 && !mySlots[0].GetComponent<ProtoPanelSlot>().RawProbe() && guardProbed) {
    //        // when the only existing slot is empty
    //        mySlots[0].GetComponent<ProtoPanelSlot>().ResetTimer();
    //        guardProbed = false;
    //        return;
    //    }

    //    // check probe recording and wipe them
    //    foreach (GameObject obj in occupiedSlotCondition) {
    //        int probedIndex = mySlots.IndexOf(obj);
    //        // the only purpose of this loop is to generate new slots

    //        // if the probed slot is adjacent to some open slot
    //        if (probedIndex - 1 >= 0 && !mySlots[probedIndex - 1].GetComponent<ProtoPanelSlot>().RawProbe()) {
    //            if (unoccupiedSlotCondition.Contains(mySlots[probedIndex - 1]))
    //                mySlots[probedIndex - 1].GetComponent<ProtoPanelSlot>().ResetTimer();
    //            else {
    //                FormatNewSlot(probedIndex + 1);
    //            }

    //            break;
    //        }
    //        else if (probedIndex + 1 < mySlots.Count && !mySlots[probedIndex + 1].GetComponent<ProtoPanelSlot>().RawProbe()) {
    //            if (unoccupiedSlotCondition.Contains(mySlots[probedIndex + 1]))
    //                mySlots[probedIndex + 1].GetComponent<ProtoPanelSlot>().ResetTimer();
    //            else {
    //                FormatNewSlot(probedIndex);
    //            }

    //            break;
    //        }
    //        // otherwise find the correct direction and insert the new slot
    //        // in this prototype I assume there's at most only 2 slots probed at same time
    //        else if (probedIndex - 1 >= 0 && occupiedSlotCondition.Contains(mySlots[probedIndex - 1])) {
    //            FormatNewSlot(probedIndex);
    //            break;
    //        }
    //        else {
    //            FormatNewSlot(probedIndex + 1);
    //            break;
    //        }
    //    }

    //    foreach (GameObject slot in unoccupiedSlotCondition) {
    //        slot.GetComponent<ProtoPanelSlot>().ResetTimer();
    //    }



    //    occupiedSlotCondition.Clear();
    //    unoccupiedSlotCondition.Clear();
    //}


    // utils ////////////////////////////////////////////////////////////////////////////////
    private GameObject FormatNewSlot(int index) {
        GameObject newSlot = Instantiate(mySlotInstance, transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.GetComponent<ProtoPanelSlot>().myPanel = this;

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
