using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProtoDraggable : MonoBehaviour, IDraggable {

    // API settings
    [SerializeField] private string myLayer;

    // permenant states
    [SerializeField] private GameObject currentSeat = null;

    // temporary states related to "drag"
    [SerializeField] private bool isDragged = false;
    private Vector2 mouseLockPos = Vector2.zero;
    private Vector2 myLockPos = Vector2.zero;

    // temporary states related to "drop"
    private GameObject candidateSeat = null;


    // UI event handlers ///////////////////////////////////////////////////////////////
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("hello");

        isDragged = true;

        // update diff
        mouseLockPos = JohnnyUITools.GetMousePosInMyCanvas(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData) {
        // update seat information
        if (candidateSeat && candidateSeat != currentSeat) {
            Debug.Log("seat found and updating");

            // lock onto new seat
            if (currentSeat) currentSeat.GetComponent<IDroppable>().ItemLeft(gameObject);
            candidateSeat.GetComponent<IDroppable>().ItemCame(gameObject);
            currentSeat = candidateSeat;

            // update lock position to new anchored position
            myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        }
        else if (currentSeat) {
            // when there's no change in seat, resume to old position
            gameObject.GetComponent<RectTransform>().anchoredPosition = myLockPos;
        }

        // reset position states
        mouseLockPos = Vector2.zero;
        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;

        isDragged = false;
    }


    // start ///////////////////////////////////////////////////////////////////////////
    private void Start() {
        // some debug messages to check if the position translation worked
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);

        // why the hell did I do this??? - johnny
        Vector3[] v = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(v);
        foreach (Vector3 i in v) {
            Debug.Log(i);
        }

        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }


    // update //////////////////////////////////////////////////////////////////////////
    public void Update() {
        if (isDragged) {
            // set rect transform position
            gameObject.GetComponent<RectTransform>().anchoredPosition =
                JohnnyUITools.GetMousePosInMyCanvas(gameObject) - mouseLockPos + myLockPos;

            // get a list of all seats / thank you Unity for the tag system
            GameObject[] seatObjects = GameObject.FindGameObjectsWithTag("Seat");
            GameObject closestSeat = null;

            // like the var name, it's dimensions of this draggable object
            Vector2 myDimensions = gameObject.GetComponent<RectTransform>().sizeDelta;

            // loop all seats for the closest overlap
            foreach (GameObject seatObject in seatObjects) {
                IDroppable seat = seatObject.GetComponent<IDroppable>();
                Vector2 seatDimensions = seatObject.GetComponent<RectTransform>().sizeDelta;

                Vector2 diff = JohnnyUITools.GetCanvasCoord(gameObject) - JohnnyUITools.GetCanvasCoord(seatObject);

                // if overlapping, and not occupied, and within the correct layer
                if (diff.x > -myDimensions.x && diff.x < seatDimensions.x
                    && diff.y > -myDimensions.y && diff.y < seatDimensions.y
                    && !seat.IsOccupied() && seat.GetLayer() == myLayer) {

                    if (closestSeat == null) {
                        // initializing candidate
                        closestSeat = seatObject;
                    }
                    else {
                        // compare candidates
                        Vector2 oldDiff = JohnnyUITools.GetCanvasCoord(gameObject)
                            - JohnnyUITools.GetCanvasCoord(closestSeat);
                        closestSeat = (diff.magnitude < oldDiff.magnitude) ? seatObject : closestSeat;
                    }
                }
            }

            // register this candidate
            candidateSeat = closestSeat;
        }
    }

    public string GetInformation() {
        return "this is an empty item with no information";
    }
}
