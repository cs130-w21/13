using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoCodingPanel : MonoBehaviour, ICodeInfo {
    [SerializeField]
    private ProtoPanelGuard myGuard;

    [SerializeField]
    private GameObject mySlotInstance;

    // used to determine positions
    private HashSet<GameObject> occupiedSlotCondition = new HashSet<GameObject>();
    private HashSet<GameObject> unoccupiedSlotCondition = new HashSet<GameObject>();

    // guard message
    private bool guardProbed = false;

    // need to make sure its order corresponds to positional order
    [SerializeField]
    private List<GameObject> mySlots = new List<GameObject>();


    // public interface ////////////////////////////////////////////////////////////////////
    public string GetInformation() {
        string newInformation = "";

        foreach (GameObject slot in mySlots) {
            if (slot.GetComponent<ProtoPanelSlot>().RawProbe()) {
                if (slot.GetComponent<ICodeInfo>().GetInformation() != "") {
                    newInformation += slot.GetComponent<ICodeInfo>().GetInformation() + " ";
                }
            }
        }

        return newInformation;
    }


    // messages with slots /////////////////////////////////////////////////////////////////
    public void OccupiedProbe(GameObject probedObject) {
        occupiedSlotCondition.Add(probedObject);
    }

    public void UnoccupiedProbe(GameObject probedObject) {
        unoccupiedSlotCondition.Add(probedObject);
    }

    public void DeprecateSlot(GameObject deprecatedSlot) {
        mySlots.Remove(deprecatedSlot);
        Destroy(deprecatedSlot);
    }


    // message with guard (handling empty) //////////////////////////////////////////////////
    public void ReportGuardProbe() {
        guardProbed = true;
    }


    // system messages //////////////////////////////////////////////////////////////////////
    public void LateUpdate() {
        if (guardProbed && mySlots.Count == 0) {
            // instantiate the first slot
            FormatNewSlot(0);
            guardProbed = false;
            return;
        }
        else if (mySlots.Count == 1 && !mySlots[0].GetComponent<ProtoPanelSlot>().RawProbe() && guardProbed) {
            // when the only existing slot is empty
            mySlots[0].GetComponent<ProtoPanelSlot>().ResetTimer();
            guardProbed = false;
            return;
        }
        
        // check probe recording and wipe them
        foreach (GameObject obj in occupiedSlotCondition) {
            int probedIndex = mySlots.IndexOf(obj);
            // the only purpose of this loop is to generate new slots

            // if the probed slot is adjacent to some open slot
            if (probedIndex - 1 >= 0 && !mySlots[probedIndex - 1].GetComponent<ProtoPanelSlot>().RawProbe()) {
                if (unoccupiedSlotCondition.Contains(mySlots[probedIndex - 1]))
                    mySlots[probedIndex - 1].GetComponent<ProtoPanelSlot>().ResetTimer();
                else {
                    FormatNewSlot(probedIndex + 1);
                }

                break;
            }
            else if (probedIndex + 1 < mySlots.Count && !mySlots[probedIndex + 1].GetComponent<ProtoPanelSlot>().RawProbe()) {
                if (unoccupiedSlotCondition.Contains(mySlots[probedIndex + 1]))
                    mySlots[probedIndex + 1].GetComponent<ProtoPanelSlot>().ResetTimer();
                else {
                    FormatNewSlot(probedIndex);
                }

                break;
            }
            // otherwise find the correct direction and insert the new slot
            // in this prototype I assume there's at most only 2 slots probed at same time
            else if (probedIndex - 1 >= 0 && occupiedSlotCondition.Contains(mySlots[probedIndex - 1])) {
                FormatNewSlot(probedIndex);
                break;
            }
            else {
                FormatNewSlot(probedIndex + 1);
                break;
            }
        }

        foreach (GameObject slot in unoccupiedSlotCondition) {
            slot.GetComponent<ProtoPanelSlot>().ResetTimer();
        }



        occupiedSlotCondition.Clear();
        unoccupiedSlotCondition.Clear();
    }


    // utils ////////////////////////////////////////////////////////////////////////////////
    private GameObject FormatNewSlot(int index) {
        GameObject newSlot = Instantiate(mySlotInstance, transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.GetComponent<ProtoPanelSlot>().myPanel = this;

        newSlot.transform.SetSiblingIndex(index);
        mySlots.Insert(index, newSlot);

        return newSlot;
    }
}
