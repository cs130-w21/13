using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProtoDrive : MonoBehaviour, IDraggable, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private IDroppable currentSeat = null;

    [SerializeField] private bool isDragged = false;

    private Vector2 currentDiff = Vector2.zero;

    public Canvas myCanvas;

    // shortcuts to mouse position translation ////////////////////////////////
    private Vector2 GetMousePosInCanvas() {
        float mousePosX = Input.mousePosition.x;
        float mousePosY = Input.mousePosition.y;

        float canvasWidth = myCanvas.GetComponent<RectTransform>().sizeDelta.x;
        float canvasHeight = myCanvas.GetComponent<RectTransform>().sizeDelta.y;

        float mousePosInCanvasX = mousePosX / Screen.width * canvasWidth;
        float mousePosInCanvasY = mousePosY / Screen.height * canvasHeight;

        return new Vector2(mousePosInCanvasX, mousePosInCanvasY);
    }

    // UI event handlers //////////////////////////////////////////////////////
    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log("hello");

        isDragged = true;

        // update diff
        currentDiff = gameObject.GetComponent<RectTransform>().anchoredPosition - GetMousePosInCanvas();
    }

    public void OnPointerUp(PointerEventData eventData) {
        isDragged = false;

        // reset diff
        currentDiff = Vector2.zero;
    }

    // start //////////////////////////////////////////////////////////////////
    private void Start() {
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        Debug.Log(myCanvas.GetComponent<RectTransform>().sizeDelta);
    }

    // update /////////////////////////////////////////////////////////////////
    public void Update() {
        if (isDragged) {
            Debug.Log("updated drag");
            // move it with the mouse

            // set rect transform position
            gameObject.GetComponent<RectTransform>().anchoredPosition = GetMousePosInCanvas() + currentDiff;
        }
    }
}
