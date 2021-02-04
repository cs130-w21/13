﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProtoPlug : MonoBehaviour, IDraggable {

    [SerializeField] private GameObject currentSeat = null;
    
    [SerializeField] private bool isDragged = false;

    [SerializeField] private string myLayer;

    // temporary states related to "drag"
    private Vector2 mouseLockPos = Vector2.zero;
    private Vector2 myLockPos = Vector2.zero;

    // temporary states related to "drop"
    private GameObject candidateSeat = null;


    // UI event handlers //////////////////////////////////////////////////////
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


    // start //////////////////////////////////////////////////////////////////
    private void Start() {
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        // Debug.Log(myContainer.GetComponent<RectTransform>().sizeDelta);
        Vector3[] v = new Vector3[4];
        GetComponent<RectTransform>().GetWorldCorners(v);
        foreach (Vector3 i in v) {
            Debug.Log(i);
        }

        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }


    // update /////////////////////////////////////////////////////////////////
    public void Update() {
        if (isDragged) {
            // set rect transform position
            gameObject.GetComponent<RectTransform>().anchoredPosition = 
                JohnnyUITools.GetMousePosInMyCanvas(gameObject) - mouseLockPos + myLockPos;

            // finding the right "seat" to fit in
            GameObject[] seatObjects = GameObject.FindGameObjectsWithTag("Seat");
            GameObject closestSeat = null;

            Vector2 myDimensions = gameObject.GetComponent<RectTransform>().sizeDelta;

            // loop all seats for an overlap (or the closest overlap)
            foreach (GameObject seatObject in seatObjects) {
                IDroppable seat = seatObject.GetComponent<IDroppable>();
                Vector2 seatDimensions = seatObject.GetComponent<RectTransform>().sizeDelta;

                Vector2 diff = JohnnyUITools.GetCanvasCoord(gameObject) - JohnnyUITools.GetCanvasCoord(seatObject);

                // if overlapping, and not occupied, and under the same layer
                if (diff.x > - myDimensions.x && diff.x < seatDimensions.x 
                    && diff.y > - myDimensions.y && diff.y < seatDimensions.y
                    && !seat.IsOccupied() && seat.GetLayer() == myLayer) {
                    if (closestSeat == null) {
                        // when this is the only seat so far
                        closestSeat = seatObject;
                    }
                    else {
                        // overlapping with multiple: choose the closer one
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
}
