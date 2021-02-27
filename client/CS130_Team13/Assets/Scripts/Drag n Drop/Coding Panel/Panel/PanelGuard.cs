using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGuard : MonoBehaviour, IDroppable {
    [SerializeField]
    private string myLayer;

    [SerializeField]
    private CodingPanel myPanel;
    
    public virtual GameObject GetCurrentItem() {
        throw new System.NotImplementedException();
    }

    public virtual string GetLayer() {
        return myLayer;
    }

    public virtual bool IsOccupied() {
        return !myPanel.ReportGuardProbe();
    }

    public virtual void ItemCame(GameObject item) {
        // send item to panel
        myPanel.PutItem(item);
    }

    public virtual void ItemLeft(GameObject item) {
        Debug.Log("item passed to panel");
    }

    private void Update() {
        gameObject.GetComponent<RectTransform>().anchoredPosition = myPanel.GetComponent<RectTransform>().anchoredPosition;
        gameObject.GetComponent<RectTransform>().sizeDelta = myPanel.GetComponent<RectTransform>().sizeDelta;
    }
}
