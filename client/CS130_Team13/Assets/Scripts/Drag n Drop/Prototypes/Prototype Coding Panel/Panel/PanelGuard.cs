using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGuard : MonoBehaviour, IDroppable {
    [SerializeField]
    private string myLayer;

    public CodingPanel myPanel;
    
    public GameObject GetCurrentItem() {
        throw new System.NotImplementedException();
    }

    public string GetLayer() {
        return myLayer;
    }

    public bool IsOccupied() {
        return !myPanel.ReportGuardProbe();
    }

    public void ItemCame(GameObject item) {
        // send item to panel
        myPanel.PutItem(item);
    }

    public void ItemLeft(GameObject item) {
        Debug.Log("item passed to panel");
    }

    private void Update() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = myPanel.GetComponent<RectTransform>().anchoredPosition;
        gameObject.GetComponent<RectTransform>().sizeDelta = myPanel.GetComponent<RectTransform>().sizeDelta;
    }
}
