using MessagePipe;
using System;

public class DropZoneSystem
{
    private readonly ISubscriber<FigureActionMessage.BaseFigureObjectOut> _figureObjectOut;

    private IDraggingService _draggingService;
    private IDisposable _subscription;
    public void Dispose() => _subscription?.Dispose();

    public DropZoneSystem(ISubscriber<FigureActionMessage.BaseFigureObjectOut> figureObjectOut,
                          IDraggingService draggingService)
    {
        _figureObjectOut = figureObjectOut;
        _draggingService = draggingService;

        InitializeMessages();
        _draggingService = draggingService;
    }

    private void InitializeMessages()
    {
        var bag = DisposableBag.CreateBuilder();

        _figureObjectOut.Subscribe(_ => DeleteFigure()).AddTo(bag);

        _subscription = bag.Build();
    }

    private void DeleteFigure()
    {
        _draggingService.CurrentDraggingCopy.Value.DestroyWithAnimation();
    }   
}
