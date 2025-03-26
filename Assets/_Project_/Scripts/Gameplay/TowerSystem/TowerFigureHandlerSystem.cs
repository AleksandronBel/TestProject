using DG.Tweening;
using MessagePipe;
using UnityEngine;
using R3;
using System;
using ObservableCollections;
using Cysharp.Threading.Tasks;

public class TowerFigureHandlerSystem : IDisposable
{
    private readonly ISubscriber<FigureStatesMessage.FigureForBuildTowerDragEnd> _figureForBuildTowerDragEnd;
    private readonly ISubscriber<FigureStatesMessage.DraggingObjectOutFromTower> _draggingObjectOutFromTower;
    private readonly ISubscriber<TowerMessages.TowerResetData> _towerResetData;

    private readonly IPublisher<FigureActionMessage.FigureAction> _figureActionMessage;

    private readonly ObservableList<DraggingObject> _stackedObjects = new();
    public IReadOnlyObservableList<DraggingObject> StackedObjects => _stackedObjects;

    private Transform _parent;

    public ReactiveProperty<float> MaxHeight => _maxHeight;
    public ReactiveProperty<float> CurrentTowerHeight => _currentTowerHeight;

    private ReactiveProperty<float> _maxHeight = new();
    private ReactiveProperty<float> _currentTowerHeight = new();
    private IDisposable _subscription;

    private IDraggingService _draggingService;
    private IFigureProvider _figureProvider;

    private GameFactory _gameFactory;
    //private SaveSystem _saveSystem;

    public void Dispose() => _subscription?.Dispose();

    public TowerFigureHandlerSystem(IDraggingService draggingService,
                                    IFigureProvider figureProvider,
                                    ISubscriber<FigureStatesMessage.FigureForBuildTowerDragEnd> figureForBuildTowerDragEnd,
                                    ISubscriber<FigureStatesMessage.DraggingObjectOutFromTower> draggingObjectOutFromTower,
                                    ISubscriber<TowerMessages.TowerResetData> towerResetData,
                                    IPublisher<FigureActionMessage.FigureAction> figureActionMessage,
                                    GameFactory gameFactory,
                                    /*SaveSystem saveSystem*/)
    {
        _draggingService = draggingService;
        _figureProvider = figureProvider;

        _figureForBuildTowerDragEnd = figureForBuildTowerDragEnd;
        _draggingObjectOutFromTower = draggingObjectOutFromTower;
        _towerResetData = towerResetData;

        _figureActionMessage = figureActionMessage;

        _gameFactory = gameFactory;
        //_saveSystem = saveSystem;

        InitializeMessages();

        LoadTower();
    }

    private void InitializeMessages()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();

        _figureForBuildTowerDragEnd.Subscribe(SetFigure).AddTo(bag);
        _draggingObjectOutFromTower.Subscribe(DeleteFigure).AddTo(bag);
        _towerResetData.Subscribe(_ => ClearTower()).AddTo(bag);

        _subscription = bag.Build();
    }

    public void SetFigure(FigureStatesMessage.FigureForBuildTowerDragEnd message)
    {
        var eventData = message.PointerEventData;

        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out BaseFigureView figureView))
        {
            var item = _draggingService.CurrentDraggingCopy.Value;

            var transform = message.Transform;
            var rectTransform = message.RectTransform;

            _parent = transform;

            if (StackedObjects.Count == 0)
            {
                float screenHeight = rectTransform.rect.height;
                float itemHeight = item.RectTransform.rect.height;
                float itemY = item.RectTransform.anchoredPosition.y;
                float itemPivot = item.RectTransform.pivot.y;
                float itemBottomY = (screenHeight * (1 - rectTransform.pivot.y)) + itemY - (itemHeight * itemPivot);

                _maxHeight.Value = screenHeight - itemBottomY;
            }

            if (CanPlaceOnTower(item.RectTransform.position, item) && CheckTowerHeight(item))
                PlaceOnTower(item);
            else
                item.DestroyWithAnimation();
        }
    }

    private bool CanPlaceOnTower(Vector3 position, DraggingObject item)
    {
        if (StackedObjects.Count == 0) return true;

        var lastBlock = StackedObjects[StackedObjects.Count - 1];

        bool isAbove = position.y > lastBlock.RectTransform.position.y;

        float halfBlockSize = item.RectTransform.rect.height * 0.5f;
        bool isAlignedHorizontally = Mathf.Abs(position.x - lastBlock.RectTransform.position.x) < halfBlockSize;

        bool isFigureHasSameColor = lastBlock.Image.sprite == item.Image.sprite; //for possible future code expansion (color matching check of the cube (sprite)) 

        return isAbove && isAlignedHorizontally /*&& isFigureHasSameColor*/ ;
    }

    private bool CheckTowerHeight(DraggingObject item)
    {
        if (_currentTowerHeight.Value + item.RectTransform.rect.height >= _maxHeight.Value)
        {
            InvokeHeightTowerMessage();
            return false;
        }

        return true;
    }

    private async void InvokeHeightTowerMessage()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        _figureActionMessage.Publish(new(MessageFigureType.tower_height_limit));
    }

    private void PlaceOnTower(DraggingObject item)
    {
        float blockHeight = item.RectTransform.rect.height;
        _currentTowerHeight.Value += blockHeight;

        item.IsFigureInTower = true;

        Vector2 newPosition;

        if (StackedObjects.Count == 0)
        {
            newPosition = item.RectTransform.position;
            item.transform.SetParent(_parent, worldPositionStays: true);
            _stackedObjects.Add(item);

            SaveTower();
        }
        else
        {
            var lastBlock = StackedObjects[StackedObjects.Count - 1];
            Vector3 lastBlockLocalPosition = _parent.InverseTransformPoint(lastBlock.RectTransform.position);
            float xOffset = UnityEngine.Random.Range(-blockHeight * 0.5f, blockHeight * 0.5f);
            newPosition = new Vector3(lastBlockLocalPosition.x + xOffset, lastBlockLocalPosition.y + blockHeight, 0);
            Vector3 worldPosition = _parent.TransformPoint(newPosition);

            _stackedObjects.Add(item);

            item.RectTransform.DOMove(worldPosition, 0.3f).SetEase(Ease.OutBounce).OnKill(() =>
            {
                SaveTower();
            });

            item.transform.SetParent(_parent, worldPositionStays: true);
        }

        _figureActionMessage.Publish(new(MessageFigureType.figure_placed));
    }

    public void RemoveFigure(DraggingObject draggingObject)
    {
        int removedIndex = _stackedObjects.IndexOf(draggingObject);
        if (removedIndex == -1) return;

        _stackedObjects.RemoveAt(removedIndex);

        float localHeight = draggingObject.RectTransform.rect.height;

        float worldHeight = draggingObject.RectTransform.TransformVector(0, localHeight, 0).y;

        Sequence sequence = DOTween.Sequence();

        for (int i = removedIndex; i < StackedObjects.Count; i++)
        {
            var currentObject = StackedObjects[i];

            sequence.Append(currentObject.RectTransform.DOMoveY(
                currentObject.RectTransform.position.y - worldHeight,
                0.3f
            ).SetEase(Ease.OutBounce));
        }

        sequence.OnKill(() => SaveTower());
    }

    public void DeleteFigure(FigureStatesMessage.DraggingObjectOutFromTower message)
    {
        var draggingObject = message.Figure;
            
        RemoveFigure(draggingObject);

        draggingObject.IsDraggingObjectRemoved = true;

        CurrentTowerHeight.Value -= draggingObject.RectTransform.rect.height;

        draggingObject.DestroyWithAnimation();

        _figureActionMessage.Publish(new(MessageFigureType.figure_out));
    }

    /*public void LoadFigure(FigureSaveData figureData)
    {
        var figure = _figureProvider.GetFigureById(figureData.SpriteId);

        if (figure == null)
        {
            Debug.LogError($"Не удалось загрузить фигуру с ID {figureData.SpriteId}");
            return;
        }

        var draggingObject = _gameFactory.Instantiate(figure.DraggingObjectPrefab, _parent);

        if (draggingObject == null)
        {
            Debug.LogError("Ошибка при создании DraggingObject");
            return;
        }

        draggingObject.transform.SetParent(_parent, worldPositionStays: true);
        draggingObject.RectTransform.anchoredPosition = new Vector2(figureData.X, figureData.Y);
        draggingObject.IsFigureInTower = true;

        _stackedObjects.Add(draggingObject);
    }

    private void SaveTower()
    {
        _saveSystem.Save();
    }

    private void LoadTower()
    {
        _saveSystem.Load();
    }

    public void ClearTower()
    {
        _saveSystem.Reset();

        foreach (var figure in _stackedObjects)
            figure.DestroyWithAnimation();

        _stackedObjects.Clear();

        CurrentTowerHeight.Value = 0;
        _maxHeight.Value = 0;

        SaveTower();
    }*/
}
