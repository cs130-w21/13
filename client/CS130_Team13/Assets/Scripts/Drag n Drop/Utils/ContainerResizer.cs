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
        RectTransform[] children = transform.GetComponentsInChildren<RectTransform>();

        float height = 0;

        foreach (RectTransform rt in children) {
            height += rt.sizeDelta.y * rt.localScale.y;
        }

        // update self
        gameObject.GetComponent<RectTransform>().sizeDelta.Set(gameObject.GetComponent<RectTransform>().sizeDelta.x, height);
    }
}
