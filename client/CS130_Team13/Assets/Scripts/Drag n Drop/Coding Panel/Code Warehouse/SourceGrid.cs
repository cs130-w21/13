using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attach to game object and 
/// assign a IDraggable instance to its itemPrefab field to make it a generator of the assigned prefab;
/// </summary>
public class SourceGrid : DroppablePhysics {
    private GameObject myItem = null;

    [SerializeField]
    private GameObject itemPrefab;

    public override void ItemCame(GameObject item) {
        if (myItem != null) {
            Destroy(item);
        }
        else {
            myItem = item;
            base.ItemCame(item);
        }
    }

    public override void ItemLeft(GameObject item) {
        StartCoroutine(Count2Frames(item));
    }

    public override void Start() {
        base.Start();

        if (myItem == null) {
            Debug.Log("adding new item");
            ItemLeft(null);
        }
    }


    // my ugly workaround
    private IEnumerator Count2Frames(GameObject item) {
        for (int i = 0; i < 2; i++) {
            yield return null;
        }

        myItem = null;
        GameObject newItem = Instantiate(itemPrefab, transform);
        newItem.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        newItem.GetComponent<IDraggable>().ForceInto(gameObject);
    }
}
