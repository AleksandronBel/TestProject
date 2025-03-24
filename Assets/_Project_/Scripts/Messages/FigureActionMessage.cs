using MessagePipe;
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
    public readonly struct FigurePlaced : IMessage
    {
        public MessageType MessageType { get; }
        public FigurePlaced(MessageType messageType) => MessageType = messageType;
    }

    public readonly struct FigureOut : IMessage
    {
        public MessageType MessageType { get; }
        public FigureOut(MessageType messageType) => MessageType = messageType;
    }

    public readonly struct FigureDisappear : IMessage
    {
        public MessageType MessageType { get; }
        public FigureDisappear(MessageType messageType) => MessageType = messageType;
    }

    public readonly struct TowerHeightLimit : IMessage
    {
        public MessageType MessageType { get; }
        public TowerHeightLimit(MessageType messageType) => MessageType = messageType;
    }

    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<FigurePlaced>(options);
        container.BindMessageBroker<FigureOut>(options);
        container.BindMessageBroker<FigureDisappear>(options);
        container.BindMessageBroker<TowerHeightLimit>(options);
    }
}