using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TowerBuildZone : MonoBehaviour, IDropHandler
{
    [Inject] IPublisher<FigureStatesMessage.FigureForBuildTowerDragEnd> _figureForBuildTowerDragEnd;

    [SerializeField] private RectTransform _canvasRectTransform;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out BaseFigureView figureView))
            _figureForBuildTowerDragEnd.Publish(new(eventData, transform, _canvasRectTransform));
    }
}