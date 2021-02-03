using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProtoDrive : MonoBehaviour, IDraggable, IPointerDownHandler, IPointerUpHandler {

    [SerializeField] private IDroppable currentSeat = null;
    
    [SerializeField] private bool isDragged = false;

    private Vector2 currentDiff = Vector2.zero;
    private Vector2 mouseLockPos = Vector2.zero;
    private Vector2 myLockPos = Vector2.zero;

    // public GameObject myContainer;
    public Canvas myCanvas;

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
        // currentDiff = gameObject.GetComponent<RectTransform>().anchoredPosition - myContainer.GetComponent<IContainer>().GetMousePosInMe();
        mouseLockPos = GetMousePosInCanvas();
    }

    public void OnPointerUp(PointerEventData eventData) {
        isDragged = false;

        // reset diff
        currentDiff = Vector2.zero;
        mouseLockPos = Vector2.zero;
        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }


    // start //////////////////////////////////////////////////////////////////
    private void Start() {
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        // Debug.Log(myContainer.GetComponent<RectTransform>().sizeDelta);

        myLockPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
    }


    // update /////////////////////////////////////////////////////////////////
    public void Update() {
        if (isDragged) {
            Debug.Log("updated drag");
            // move it with the mouse

            // set rect transform position
            // gameObject.GetComponent<RectTransform>().anchoredPosition = myContainer.GetComponent<IContainer>().GetMousePosInMe() + currentDiff;
            gameObject.GetComponent<RectTransform>().anchoredPosition = GetMousePosInCanvas() - mouseLockPos + myLockPos;
        }
    }
}
