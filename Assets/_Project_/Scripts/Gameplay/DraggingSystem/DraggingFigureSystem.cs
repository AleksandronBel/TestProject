using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using System;

public class DraggingFigureSystem : IDisposable
{
    private readonly ISubscriber<FigureStatesMessage.BaseFigureDragStart> _baseFigureDragStart;
    private readonly ISubscriber<FigureStatesMessage.FigureDragging> _figureDragging;
    private readonly ISubscriber<FigureStatesMessage.BaseFigureDragEnd> _baseFigureDragEnd;

    private readonly GameFactory _gameFactory;
    private IDraggingService _draggingService;
    private IDisposable _subscription;
    public void Dispose() => _subscription?.Dispose();

    public DraggingFigureSystem(ISubscriber<FigureStatesMessage.BaseFigureDragStart> baseFigureDragStart,
                                ISubscriber<FigureStatesMessage.FigureDragging> figureDragging,
                                ISubscriber<FigureStatesMessage.BaseFigureDragEnd> basefigureDragEnd,
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

    private void StartDrag(FigureStatesMessage.BaseFigureDragStart message)
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

    private void Dragging(FigureStatesMessage.FigureDragging message)
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
}