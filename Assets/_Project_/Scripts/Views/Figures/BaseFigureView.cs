using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public abstract class BaseFigureView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Inject] IPublisher<FigureStatesMessage.BaseFigureDragStart> _baseFigureDragStart;
    [Inject] IPublisher<FigureStatesMessage.FigureDragging> _figureDragging;
    [Inject] IPublisher<FigureStatesMessage.BaseFigureDragEnd> _baseFigureDragEnd;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _imageFigureColor;
    [SerializeField] private DraggingObject _draggingObjectPrefab;

    public DraggingObject DraggingObjectPrefab => _draggingObjectPrefab;

    public Canvas MainCanvas { get; set; }
    public Image ImageFigureColor => _imageFigureColor;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _baseFigureDragStart.Publish(new(this));
    }

    public void OnDrag(PointerEventData eventData)
    {
        _figureDragging.Publish(new(eventData));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _baseFigureDragEnd.Publish(new());
    }
}
