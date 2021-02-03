using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnnyUITools
{
    /// <summary>
    /// find the "global" position of game object in terms of its canvas
    /// </summary>
    /// <param name="gameObject">game object whose position to find</param>
    /// <returns>Vector2: difference vector from canvas lower left corner to the input object</returns>
    public static Vector2 GetCanvasCoord(GameObject gameObject) {
        Vector2 result = Vector2.zero;

        GameObject me = gameObject;
        GameObject parent = gameObject.GetComponent<RectTransform>().parent.gameObject;

        // while it's not directly under canvas yet
        while (parent.GetComponent<Canvas>() == null) {
            Vector2 myCenterPos = me.GetComponent<RectTransform>().anchoredPosition;
            Vector2 myDimension = me.GetComponent<RectTransform>().sizeDelta;
            Vector2 myPivot = me.GetComponent<RectTransform>().pivot;

            result += myCenterPos - new Vector2(myDimension.x * myPivot.x, myDimension.y * myPivot.y);

            me = parent;
            parent = me.GetComponent<RectTransform>().parent.gameObject;
        }

        return result;
    }


    /// <summary>
    /// return the canvas the input game object is currently under
    /// </summary>
    /// <param name="gameObject">game object to start the search</param>
    /// <returns>Canvas: the canvas that contains input object</returns>
    public static Canvas GetMyCanvas(GameObject gameObject) {
        GameObject canvasObj = gameObject;
        for (; canvasObj.GetComponent<Canvas>() == null; canvasObj = canvasObj.GetComponent<RectTransform>().parent.gameObject) { }
        return canvasObj.GetComponent<Canvas>();
    }


    /// <summary>
    /// find mouse position under the gameobject's canvas
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns>Vector2: mouse position</returns>
    public static Vector2 GetMousePosInMyCanvas(GameObject gameObject) {
        // find the canvas I'm currently under
        Canvas myCanvas = JohnnyUITools.GetMyCanvas(gameObject);

        // calculate mouse's position using this canvas' coordinates (anchor = lower left corner)
        float mousePosX = Input.mousePosition.x;
        float mousePosY = Input.mousePosition.y;

        float canvasWidth = myCanvas.GetComponent<RectTransform>().sizeDelta.x;
        float canvasHeight = myCanvas.GetComponent<RectTransform>().sizeDelta.y;

        float mousePosInCanvasX = mousePosX / Screen.width * canvasWidth;
        float mousePosInCanvasY = mousePosY / Screen.height * canvasHeight;

        return new Vector2(mousePosInCanvasX, mousePosInCanvasY);
    }
}
