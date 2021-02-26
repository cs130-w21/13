using UnityEngine.EventSystems;
using UnityEngine;

public interface IDraggable : IPointerDownHandler, IPointerUpHandler {
    void ForceInto(GameObject droppable);
}
