using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProtoPanelSlot : ProtoDroppable
{
    public ProtoCodingPanel myPanel;

    /// <summary>
    /// frames the slot remains while not being probed
    /// </summary>
    [SerializeField]
    private float waitDuration = 1;

    [SerializeField]
    private bool foolSwtich = false;
    
    private float timer = 0;
    
    public override bool IsOccupied() {
        bool isOccupied = base.IsOccupied();

        // if it's already occupied, tell the panel it's being probed
        if (isOccupied && myPanel) {
            myPanel.OccupiedProbe(gameObject);
        }
        else if (!isOccupied) {
            myPanel.UnoccupiedProbe(gameObject);
        }

        return isOccupied;
    }

    public bool RawProbe() {
        return base.IsOccupied();
    }

    public void ResetTimer() {
        timer = 0;
    }

    public void Update() {
        // if it's empty and not probed, report to panel
        if (!base.IsOccupied()) {
            timer += 1;
            if (timer >= waitDuration && !foolSwtich)
                myPanel.DeprecateSlot(gameObject);
        }
        // otherwise reset timer
        else {
            timer = 0;
        }
    }
}
