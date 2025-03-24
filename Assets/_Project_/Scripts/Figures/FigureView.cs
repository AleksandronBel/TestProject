using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class FigureView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Inject] IPublisher<FigureActionMessage.FigurePlaced> _figureDragStart;

    [SerializeField] private RectTransform _rectTrasnform;
    [SerializeField] private Image _imageFigureColor;
    [SerializeField] private DraggingObject _draggingObject;

    public Canvas MainCanvas {get; set;}
    public Image ImageFigureColor => _imageFigureColor;

    private IFigureService _figureService;

    [Inject]
    private void Construct(IFigureService figureService)
    {
        _figureService = figureService;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _figureService.StartDrag(_draggingObject, MainCanvas, _imageFigureColor, transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _figureService.Dragging(eventData, MainCanvas);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _figureService.EndDragging();
    }
}
