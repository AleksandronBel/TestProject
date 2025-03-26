using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TowerBuildZone : MonoBehaviour, IDropHandler
{
    [Inject] IPublisher<FigureStatesMessage.FigureForBuildTowerDragEnd> _figureForBuildTowerDragEnd;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private DraggingObject _draggingObjectPrefab;

    [SerializeField] private RectTransform _canvasRectTransform;

    private TowerFigureHandlerSystem _towerFigureHandlerSystem;

    [Inject]
    private void Construct(TowerFigureHandlerSystem towerFigureHandlerSystem)
    {
        _towerFigureHandlerSystem = towerFigureHandlerSystem;
        _towerFigureHandlerSystem.LoadTowerData(_draggingObjectPrefab, transform, _canvas);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out BaseFigureView figureView))
            _figureForBuildTowerDragEnd.Publish(new(eventData, transform, _canvasRectTransform));
    }
}