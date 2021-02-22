using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoCodingPanel : MonoBehaviour
{
    [SerializeField]
    private ProtoPanelGuard myGuard;

    [SerializeField]
    private GameObject mySlotInstance;

    // used to determine positions
    private HashSet<GameObject> occupiedSlotCondition = new HashSet<GameObject>();
    private HashSet<GameObject> unoccupiedSlotCondition = new HashSet<GameObject>();

    // need to make sure its order corresponds to positional order
    [SerializeField]
    private List<GameObject> mySlots = new List<GameObject>(); 

    public void OccupiedProbe(GameObject probedObject) {
        if (probedObject == myGuard) {
            // handle situation of no slot present
            // there's no implementation for this yet
        }
        else {
            occupiedSlotCondition.Add(probedObject);
        }
    }

    public void UnoccupiedProbe(GameObject probedObject) {
        unoccupiedSlotCondition.Add(probedObject);
    }

    public void DeprecateSlot(GameObject deprecatedSlot) {
        mySlots.Remove(deprecatedSlot);
        Destroy(deprecatedSlot);
    }

    public void LateUpdate() {
        // check probe recording and wipe them
        foreach (GameObject obj in occupiedSlotCondition) {
            int probedIndex = mySlots.IndexOf(obj);
            // the only purpose of this loop is to generate new slots

            // if the probed slot is adjacent to some open slot
            if ((probedIndex - 1 >= 0 && 
                !mySlots[probedIndex - 1].GetComponent<ProtoPanelSlot>().RawProbe()) 
                || (probedIndex + 1 < mySlots.Count &&
                !mySlots[probedIndex + 1].GetComponent<ProtoPanelSlot>().RawProbe())) {
                continue;
            }

            // in this prototype I assume there's at most only 2 slots probed at same time
            // otherwise find the correct direction and insert the new slot
            if (probedIndex - 1 >= 0 &&
               occupiedSlotCondition.Contains(mySlots[probedIndex - 1])) {
                FormatNewSlot(probedIndex);
                break;
            }
            else {
                FormatNewSlot(probedIndex + 1);
                break;
            }
        }

        occupiedSlotCondition.Clear();
        unoccupiedSlotCondition.Clear();
    }


    private GameObject FormatNewSlot(int index) {
        GameObject newSlot = Instantiate(mySlotInstance, transform);
        newSlot.transform.localScale = new Vector3(1, 1, 1);
        newSlot.GetComponent<ProtoPanelSlot>().myPanel = this;

        newSlot.transform.SetSiblingIndex(index);
        mySlots.Insert(index, newSlot);

        return newSlot;
    }
}
