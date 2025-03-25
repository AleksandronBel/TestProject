using DG.Tweening;
using MessagePipe;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Zenject;
using R3;
using System;

public class TowerFigureHandlerSystem : IDisposable
{
    private readonly ISubscriber<FigureActionMessage.FigureForBuildTowerDragEnd> _figureForBuildTowerDragEnd;
    private readonly ISubscriber<FigureActionMessage.DraggingObjectOutFromTower> _draggingObjectOutFromTower;

    private readonly List<DraggingObject> _stackedObjects = new();
    public IReadOnlyList<DraggingObject> StackedObjects => _stackedObjects;

    private Transform _parent;

    public ReactiveProperty<float> MaxHeight => _maxHeight;
    public ReactiveProperty<float> CurrentTowerHeight => _currentTowerHeight;

    private ReactiveProperty<float> _maxHeight = new();
    private ReactiveProperty<float> _currentTowerHeight = new();

    private IDraggingService _draggingService;
    private IDisposable _subscription;
    public void Dispose() => _subscription?.Dispose();

    public TowerFigureHandlerSystem(IDraggingService draggingService,
                                    ISubscriber<FigureActionMessage.FigureForBuildTowerDragEnd> figureForBuildTowerDragEnd,
                                    ISubscriber<FigureActionMessage.DraggingObjectOutFromTower> draggingObjectOutFromTower,
                                    GameFactory gameFactory)
    {
        _draggingService = draggingService;
        _figureForBuildTowerDragEnd = figureForBuildTowerDragEnd;
        _draggingObjectOutFromTower = draggingObjectOutFromTower;

        InitializeMessages();
    }

    private void InitializeMessages()
    {
        var bag = MessagePipe.DisposableBag.CreateBuilder();

        _figureForBuildTowerDragEnd.Subscribe(SetFigure).AddTo(bag);
        _draggingObjectOutFromTower.Subscribe(DeleteFigure).AddTo(bag);

        _subscription = bag.Build();
    }

    public void SetFigure(FigureActionMessage.FigureForBuildTowerDragEnd message)
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
            {
                PlaceOnTower(item);
            }
            else
            {
                //_figureDisappear.Publish(new(MessageType.figure_disappear));
                item.DestroyWithAnimation();
            }
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
            //_figureHeightLimit.Publish(new(MessageType.tower_height_limit));

            item.DestroyWithAnimation();
            return false;
        }

        return true;
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

            //SaveTower();
        }
        else
        {
            var lastBlock = StackedObjects[StackedObjects.Count - 1];
            Vector3 lastBlockLocalPosition = _parent.InverseTransformPoint(lastBlock.RectTransform.position);
            float xOffset = UnityEngine.Random.Range(-blockHeight * 0.5f, blockHeight * 0.5f);
            newPosition = new Vector3(lastBlockLocalPosition.x + xOffset, lastBlockLocalPosition.y + blockHeight, 0);
            Vector3 worldPosition = _parent.TransformPoint(newPosition);

            item.RectTransform.DOMove(worldPosition, 0.3f).SetEase(Ease.OutBounce).OnKill(() =>
            {
                //SaveTower();
            });

            item.transform.SetParent(_parent, worldPositionStays: true);
        }

        _stackedObjects.Add(item);
        ///_figurePlaced.Publish(new(MessageType.figure_placed));
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

        //sequence.OnKill(() => SaveTower());
    }

    public void DeleteFigure(FigureActionMessage.DraggingObjectOutFromTower message)
    {
        var draggingObject = message.Figure;

        RemoveFigure(draggingObject);

        draggingObject.IsDraggingObjectRemoved = true;

        CurrentTowerHeight.Value -= draggingObject.RectTransform.rect.height;

        draggingObject.DestroyWithAnimation();

        //_figureOut.Publish(new(MessageType.figure_out));
    }

    /*private void SaveTower()
    {
        _saveLoadProgressService.SaveTower(_stackedObjects);
    }

    public void LoadTowerData(DraggingObject draggingObject, Transform transform, Canvas canvas)
    {
        TowerSaveData data = _saveLoadProgressService.LoadTower();

        _maxHeight.Value = data.MaxHeight;
        _currentTowerHeight.Value = data.CurrentTowerHeight;

        foreach (var figureData in data.Figures)
        {
            DraggingObject newFigure = _gameFactory.Instantiate(draggingObject, transform);
            *//*newFigure.Image.sprite = draggingObject
                _spriteService.GetSprite(figureData.SpriteName);*//*

            newFigure.MainCanvas = canvas;

            newFigure.RectTransform.position = new Vector3(figureData.X, figureData.Y, 0);
            _stackedObjects.Add(newFigure);
        }
    }

    public void DeleteAllTowerData()
    {
        foreach (var figure in _stackedObjects)
            figure.DestroyWithAnimation();

        _stackedObjects.Clear();

        _currentTowerHeight.Value = 0;
        _maxHeight.Value = 0;

        SaveTower();
    }*/
}
