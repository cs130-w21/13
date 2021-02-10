using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obselete {
    /// <summary>
    /// use it with caution: the gameobject's anchor should be set at lower left corner
    /// </summary>
    public class ProtoContainer : MonoBehaviour, IContainer {
        [SerializeField] private GameObject myParentTransform;

        private Vector2 GetLowerCornerLocalPos() {
            Vector2 myCenterPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
            Vector2 myDimension = gameObject.GetComponent<RectTransform>().sizeDelta;
            Vector2 myPivot = gameObject.GetComponent<RectTransform>().pivot;

            return myCenterPos - new Vector2(myDimension.x * myPivot.x, myDimension.y * myPivot.y);
        }

        public Vector2 GetMousePosInMe() {
            if (this.IsCanvas()) {
                // return GetLowerCornerLocalPos();

                float mousePosX = Input.mousePosition.x;
                float mousePosY = Input.mousePosition.y;

                float canvasWidth = gameObject.GetComponent<RectTransform>().sizeDelta.x;
                float canvasHeight = gameObject.GetComponent<RectTransform>().sizeDelta.y;

                float mousePosInCanvasX = mousePosX / Screen.width * canvasWidth;
                float mousePosInCanvasY = mousePosY / Screen.height * canvasHeight;

                return new Vector2(mousePosInCanvasX, mousePosInCanvasY);
            }
            else {
                return myParentTransform.GetComponent<IContainer>().GetMousePosInMe() - GetLowerCornerLocalPos();
            }
        }

        public bool IsCanvas() {
            if (gameObject.GetComponent<Canvas>()) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
