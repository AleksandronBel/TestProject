using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public static class FigureStatesMessage
{
    public record BaseFigureDragStart
    {
        public BaseFigureView Figure { get; }

        public BaseFigureDragStart(BaseFigureView figure)
        {
            Figure = figure;
        }
    }

    public record FigureDragging
    {
        public PointerEventData PointerEventData;

        public FigureDragging(PointerEventData pointerEventData)
        {
            PointerEventData = pointerEventData;
        }
    }

    public record BaseFigureDragEnd();

    public record FigureForBuildTowerDragEnd
    {
        public PointerEventData PointerEventData { get; }
        public Transform Transform { get; }
        public RectTransform RectTransform { get; }

        public FigureForBuildTowerDragEnd(PointerEventData pointerEventData, Transform transform, RectTransform rectTransform)
        {
            PointerEventData = pointerEventData;
            Transform = transform;
            RectTransform = rectTransform;
        }
    }

    public record DraggingObjectOutFromTower
    {
        public DraggingObject Figure { get; }

        public DraggingObjectOutFromTower(DraggingObject figure)
        {
            Figure = figure;
        }
    }

    public record BaseFigureObjectOut
    {
        public BaseFigureView Figure { get; }

        public BaseFigureObjectOut(BaseFigureView figure)
        {
            Figure = figure;
        }
    }

    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<BaseFigureDragStart>(options);
        container.BindMessageBroker<FigureDragging>(options);
        container.BindMessageBroker<BaseFigureDragEnd>(options);
        container.BindMessageBroker<DraggingObjectOutFromTower>(options);
        container.BindMessageBroker<BaseFigureObjectOut>(options);
        container.BindMessageBroker<FigureForBuildTowerDragEnd>(options);
    }
}