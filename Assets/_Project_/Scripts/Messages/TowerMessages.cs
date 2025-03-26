using MessagePipe;
using Zenject;

public static class TowerMessages
{
    public record TowerResetData();


    public static void Install(DiContainer container, MessagePipeOptions options)
    {
        container.BindMessageBroker<TowerResetData>(options);
    }
}