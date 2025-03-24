using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class TowerHandler : MonoBehaviour, IDropHandler
{
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private RectTransform _canvasRectTransform;

    [SerializeField] private DraggingObject _draggingObject;

    private ITowerService _towerService;
    private IFigureService _figureService;

    [Inject]
    private void Construct(ITowerService towerService, IFigureService figureService)
    {
        _towerService = towerService;
        _figureService = figureService;
    }

    private void Start()
    {
        _towerService.LoadTowerData(_draggingObject, transform, _mainCanvas);
    }

    public void OnDrop(PointerEventData eventData)
    {
        _towerService.SetFigure(_figureService.CurrentDraggingCopy.Value, eventData, _canvasRectTransform, transform);
    }
}
