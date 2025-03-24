using R3;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public interface ITowerService
{
    public IReadOnlyList<DraggingObject> StackedObjects { get; }

    public ReactiveProperty<float> MaxHeight { get; }
    public ReactiveProperty<float> CurrentTowerHeight { get; }

    public void SetFigure(DraggingObject draggingObject, PointerEventData eventData, RectTransform rectTransform, Transform transform);
    public void RemoveFigure(DraggingObject draggingObject);
    public void DeleteFigure(PointerEventData eventData);

    public void LoadTowerData(DraggingObject draggingObject, Transform transform, Canvas canvas);
    public void DeleteAllTowerData();
}
