using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class DropZone : MonoBehaviour, IDropHandler
{
    private ITowerService _towerService;
    private IFigureService _figureService;

    [Inject]
    private void Construct(ITowerService towerService, IFigureService figureService)
    {
        _towerService = towerService;
        _figureService = figureService;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent(out DraggingObject draggingObject))
            _towerService.DeleteFigure(eventData);
        else if (eventData.pointerDrag.TryGetComponent(out FigureView figureView))
            _figureService.DeleteFigure(eventData);
    }
}
