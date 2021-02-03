public interface IDroppable
{
    void ItemLeft(IDraggable item);
    void ItemCame(IDraggable item);
    IDraggable GetCurrentItem();
}
