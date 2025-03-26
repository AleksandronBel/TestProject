using MessagePipe;
using Zenject;

public enum MessageFigureType
{
    figure_placed,
    figure_out,
    figure_disappear,
    tower_height_limit
}

public static class FigureActionMessage
{
    public record FigureAction : IFigureMessage
    {
        public MessageFigureType MessageType { get; }
        public FigureAction(MessageFigureType messageType) => MessageType = messageType;
    }

    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<FigureAction>(options);
    }
}