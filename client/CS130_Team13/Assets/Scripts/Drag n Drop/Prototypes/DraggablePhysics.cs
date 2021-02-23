﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePhysics : MonoBehaviour, IDraggable {

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


    /// <summary>
    /// backdoor to force this item to be attached to an IDroppable, make sure you know what you are doing when calling this
    /// </summary>
    /// <param name="droppable">the game object containing IDroppable component</param>
    public void ForceInto(GameObject droppable) {
        Debug.Log("forced");
        if (currentSeat)
            currentSeat.GetComponent<IDroppable>().ItemLeft(gameObject);

        currentSeat = droppable;
        droppable.GetComponent<IDroppable>().ItemCame(gameObject);
    }


    // UI event handlers ///////////////////////////////////////////////////////////////
    public virtual void OnPointerDown(PointerEventData eventData) {
        isDragged = true;

        // put this object on top
        if (currentSeat)
            currentSeat.GetComponent<IDroppable>().ItemLeft(gameObject);
        currentSeat = null;
        gameObject.transform.SetParent(DragDropManager.instance.GetDraggingContainer());

        // update diff
        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        mouseLockPos = JohnnyUITools.GetMousePosInMyCanvas(gameObject);
    }

    public virtual void OnPointerUp(PointerEventData eventData) {
        // update seat information
        if (candidateSeat && candidateSeat != currentSeat) {
            // lock onto new seat
            if (currentSeat) currentSeat.GetComponent<IDroppable>().ItemLeft(gameObject);
            candidateSeat.GetComponent<IDroppable>().ItemCame(gameObject);
            currentSeat = candidateSeat;

            // update lock position to new anchored position
            myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        }

        // reset position states
        mouseLockPos = Vector2.zero;
        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;

        isDragged = false;
    }


    // start ///////////////////////////////////////////////////////////////////////////
    private void Start() {
        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }


    // update //////////////////////////////////////////////////////////////////////////
    private void Update() {
        if (!isDragged && currentSeat == null && transform.parent != DragDropManager.instance.GetDraggingContainer()) {
            gameObject.transform.SetParent(DragDropManager.instance.GetDraggingContainer());
        }

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
                if (seatObject != currentSeat  && diff.x > -myDimensions.x && diff.x < seatDimensions.x
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
}
