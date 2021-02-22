using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoDroppable : MonoBehaviour, IDroppable {

    private GameObject currentItem = null;
    private RectTransform myTransform;

    [SerializeField] private string myLayer;

    
    public GameObject GetCurrentItem() {
        return currentItem;
    }


    public bool IsOccupied() {
        return currentItem != null;
    }


    private void Start() {
        myTransform = gameObject.GetComponent<RectTransform>();
    }


    public void ItemCame(GameObject item) {
        currentItem = item;

        // group under my hierarchy
        item.transform.SetParent(transform);

        // move to my center / if you read this be notified there are mutliple ways of handling "where to put it"
        RectTransform itemTransform = item.gameObject.GetComponent<RectTransform>();

        // don't use stretch anchor for this one
        itemTransform.anchoredPosition = new Vector2(
            (0.5f - itemTransform.anchorMax.x) * myTransform.sizeDelta.x,
            (0.5f - itemTransform.anchorMax.y) * myTransform.sizeDelta.y
        );
    }


    public void ItemLeft(GameObject item) {
        // release the item from this object's hierarchy
        item.transform.SetParent(JohnnyUITools.GetMyCanvas(gameObject).transform);

        currentItem = null;
    }


    public string GetLayer() {
        return myLayer;
    }
}
