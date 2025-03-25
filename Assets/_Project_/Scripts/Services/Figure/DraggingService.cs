using R3;

public class DraggingService : IDraggingService 
{
    readonly ReactiveProperty<DraggingObject> _currentDraggingCopy = new();
    public ReactiveProperty<DraggingObject> CurrentDraggingCopy => _currentDraggingCopy;
}

