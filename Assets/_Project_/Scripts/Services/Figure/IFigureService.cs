using R3;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public interface IFigureService
{
    public ReactiveProperty<DraggingObject> CurrentDraggingCopy { get; }
    public void StartDrag(DraggingObject draggingObject, Canvas canvas, Image image, Transform transform);
    public void Dragging(PointerEventData eventData, Canvas canvas);
    public void EndDragging();
    public void DeleteFigure(PointerEventData eventData);
}

