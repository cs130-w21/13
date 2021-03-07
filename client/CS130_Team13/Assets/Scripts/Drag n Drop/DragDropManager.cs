using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager instance = null;

    /// <summary>
    /// aux object that forces the dragged item to be displayed on top
    /// </summary>
    [SerializeField] private GameObject containerDuringDragging = null;

    /// <summary>
    /// allows other objects to have access to the dragged item
    /// </summary>
    public GameObject currentlyDraggedItem = null;

    /// <summary>
    /// The switch controlling if player can drag panel items.
    /// </summary>
    public bool allowDrag = true;

    private void Awake() {
        // singleton
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public Transform GetDraggingContainer() {
        return containerDuringDragging.GetComponent<Transform>();
    }
}
