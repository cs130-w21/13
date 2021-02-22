using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance = null;

    [SerializeField] private GameObject containerDuringDragging = null;

    private void Awake() {
        // singleton
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public Transform GetDraggingContainer() {
        return containerDuringDragging.GetComponent<Transform>();
    }
}
