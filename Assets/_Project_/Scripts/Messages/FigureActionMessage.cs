using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public enum MessageType
{
    figure_placed,
    figure_out,
    figure_disappear,
    tower_height_limit
}

public static class FigureActionMessage
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

    public record FigureAction : IMessage
    {
        public MessageType MessageType { get; }
        public FigureAction(MessageType messageType) => MessageType = messageType;
    }

    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<FigureAction>(options);
        container.BindMessageBroker<BaseFigureDragStart>(options);
        container.BindMessageBroker<FigureDragging>(options);
        container.BindMessageBroker<BaseFigureDragEnd>(options);
        container.BindMessageBroker<DraggingObjectOutFromTower>(options);
        container.BindMessageBroker<BaseFigureObjectOut>(options);
        container.BindMessageBroker<FigureForBuildTowerDragEnd>(options);
    }
}