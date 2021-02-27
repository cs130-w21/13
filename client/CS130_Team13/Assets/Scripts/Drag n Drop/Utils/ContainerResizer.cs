using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach this script to any item that's: 1) container that needs to update its size according to children
/// 2) contained within a layout group
/// </summary>
public class ContainerResizer : MonoBehaviour
{
    private void FixedUpdate() {
        float height = 0;

        foreach (Transform child in transform) {
            height += child.GetComponent<RectTransform>().sizeDelta.y;
        }

        // update self
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, height);
    }
}
