using MessagePipe;
using UnityEngine.EventSystems;
using UnityEngine;
using Zenject;
using R3;
using UnityEngine.UI;

public class FigureService : IFigureService
{
    [Inject] IPublisher<FigureActionMessage.FigureDisappear> _figureDisappear;
    [Inject] IPublisher<FigureActionMessage.FigureOut> _figureOut;

    readonly ReactiveProperty<DraggingObject> _currentDraggingCopy = new();
    public ReactiveProperty<DraggingObject> CurrentDraggingCopy => _currentDraggingCopy;

    private readonly GameFactory _gameFactory;

    public FigureService(GameFactory gameFactory)
    {
        _gameFactory = gameFactory;
    }

    public void StartDrag(DraggingObject draggingObject, Canvas canvas, Image image, Transform transform)
    {
        _currentDraggingCopy.Value = _gameFactory.Instantiate(draggingObject, canvas.transform);

        _currentDraggingCopy.Value.transform.position = transform.position;

        _currentDraggingCopy.Value.Image.sprite = image.sprite;
        _currentDraggingCopy.Value.MainCanvas = canvas;

        _currentDraggingCopy.Value.CanvasGroup.blocksRaycasts = false;
        _currentDraggingCopy.Value.transform.SetAsLastSibling();
    }

    public void Dragging(PointerEventData eventData, Canvas canvas)
    {
        _currentDraggingCopy.Value.RectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void EndDragging()
    {
        _currentDraggingCopy.Value.CanvasGroup.blocksRaycasts = true;

        if (!_currentDraggingCopy.Value.IsFigureInTower)
        {
            _figureDisappear.Publish(new(MessageType.figure_disappear));
            _currentDraggingCopy.Value.DestroyWithAnimation();
        }
    }

    public void DeleteFigure(PointerEventData eventData)
    {
        _currentDraggingCopy.Value.DestroyWithAnimation();
        _figureOut.Publish(new(MessageType.figure_out));
    }
}
