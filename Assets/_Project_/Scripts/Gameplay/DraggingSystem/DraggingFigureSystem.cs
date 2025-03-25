using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using System;

public class DraggingFigureSystem : IDisposable
{
    private readonly ISubscriber<FigureActionMessage.BaseFigureDragStart> _baseFigureDragStart;
    private readonly ISubscriber<FigureActionMessage.FigureDragging> _figureDragging;
    private readonly ISubscriber<FigureActionMessage.BaseFigureDragEnd> _baseFigureDragEnd;

    private readonly GameFactory _gameFactory;
    private IDraggingService _draggingService;
    private IDisposable _subscription;
    public void Dispose() => _subscription?.Dispose();

    public DraggingFigureSystem(ISubscriber<FigureActionMessage.BaseFigureDragStart> baseFigureDragStart,
                                ISubscriber<FigureActionMessage.FigureDragging> figureDragging,
                                ISubscriber<FigureActionMessage.BaseFigureDragEnd> basefigureDragEnd,
                                GameFactory gameFactory,
                                IDraggingService draggingService)
    {
        _baseFigureDragStart = baseFigureDragStart;
        _figureDragging = figureDragging;
        _baseFigureDragEnd = basefigureDragEnd;

        _gameFactory = gameFactory;
        _draggingService = draggingService;

        InitializeMessages();
    }

    private void InitializeMessages()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();

        _baseFigureDragStart.Subscribe(StartDrag).AddTo(bag);
        _figureDragging.Subscribe(Dragging).AddTo(bag);
        _baseFigureDragEnd.Subscribe(_ => EndDragging()).AddTo(bag);

        _subscription = bag.Build();
    }

    private void StartDrag(FigureActionMessage.BaseFigureDragStart message)
    {
        var baseFigure = message.Figure;
        var draggingObjectPrefab = baseFigure.DraggingObjectPrefab;

        var draggingObject = _gameFactory.Instantiate(draggingObjectPrefab, baseFigure.MainCanvas.transform);

        draggingObject.transform.position = baseFigure.transform.position;

        draggingObject.Image.sprite = baseFigure.ImageFigureColor.sprite;
        draggingObject.MainCanvas = baseFigure.MainCanvas;

        draggingObject.CanvasGroup.blocksRaycasts = false;
        draggingObject.transform.SetAsLastSibling();

        _draggingService.CurrentDraggingCopy.Value = draggingObject;    
    }

    private void Dragging(FigureActionMessage.FigureDragging message)
    {
        var eventData = message.PointerEventData;
        var draggingObject = _draggingService.CurrentDraggingCopy.Value;

        draggingObject.RectTransform.anchoredPosition += eventData.delta / draggingObject.MainCanvas.scaleFactor;
    }

    private async void EndDragging()
    {
        var draggingObject = _draggingService.CurrentDraggingCopy.Value;

        draggingObject.CanvasGroup.blocksRaycasts = true;

        await UniTask.Yield();

        if (!draggingObject.IsFigureInTower)
            draggingObject.DestroyWithAnimation();
    }

    /*public void DeleteFigure(PointerEventData eventData)
    {
        _currentDraggingCopy.Value.DestroyWithAnimation();
        //_figureOut.Publish(new(MessageType.figure_out));
    }*/
}
