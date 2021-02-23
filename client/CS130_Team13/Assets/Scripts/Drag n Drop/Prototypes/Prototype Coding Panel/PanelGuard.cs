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
        myPanel.ReportGuardProbe();
        return true;
    }

    public void ItemCame(GameObject item) {
        throw new System.NotImplementedException();
    }

    public void ItemLeft(GameObject item) {
        throw new System.NotImplementedException();
    }

    private void Update() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = myPanel.GetComponent<RectTransform>().anchoredPosition;
        gameObject.GetComponent<RectTransform>().sizeDelta = myPanel.GetComponent<RectTransform>().sizeDelta;
    }
}
