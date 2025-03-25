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
    public record FigureAction : IMessage
    {
        public MessageType MessageType { get; }
        public FigureAction(MessageType messageType) => MessageType = messageType;
    }

    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<FigureAction>(options);
    }
}