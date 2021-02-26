using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForLoopPanel : CodingPanel
{
    [SerializeField]
    private List<GameObject> myStatements = new List<GameObject>();
    private RectTransform myTransform = null;
    private float overheadLength = 0;

    private void Start() {
        // load my transform
        myTransform = gameObject.GetComponent<RectTransform>();
        overheadLength = myTransform.sizeDelta.y;
    }

    // place holder composite interface
    // public virtual string GetInformation()
    // {   
    //     // TODO: add conditions and for loop structure in result
    //     string result = "";

    //     foreach (GameObject g in myStatements) {
    //         result += g.GetComponent<ICodeInfo>().GetInformation() + "; ";
    //     }

    //     return result;
    // }

    // // place holder composite interface
    // public virtual int GetCost()
    // {
    //     int cost = 0;

    //     foreach (GameObject g in myStatements) {
    //         cost += g.GetComponent<ICodeInfo>().GetCost();
    //     }

    //     return cost;
    // }


    // // handle length
    // private void UpdateLength() {
    //     Vector2 newLength = myTransform.sizeDelta;
    //     newLength.y = 0;
        
    //     foreach (GameObject g in myStatements) {
    //         newLength.y += g.GetComponent<RectTransform>().sizeDelta.y;
    //     }

    //     myTransform.sizeDelta = newLength;
    // }

    
}
