using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoSeat : IDroppable {

    private IDraggable currentItem = null;

    public IDraggable GetCurrentItem() {
        return currentItem;
    }

    public void ItemCame(IDraggable item) {
        currentItem = item;
    }

    public void ItemLeft(IDraggable item) {
        currentItem = null;
    }
}
