using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Inject] IPublisher<FigureStatesMessage.DraggingObjectOutFromTower> _draggingObjectOutFromTower;
    [Inject] IPublisher<FigureStatesMessage.BaseFigureObjectOut> _figureObjectOut;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out DraggingObject draggingObject))
            _draggingObjectOutFromTower.Publish(new(draggingObject));

        if (eventData.pointerDrag.TryGetComponent(out BaseFigureView figureView))
            _figureObjectOut.Publish(new(figureView));
    }
}
