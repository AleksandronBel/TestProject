using MessagePipe;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    MessagePipeOptions messagePipeOptions;

    public override void InstallBindings()
    {
        BindDraggingSystem();
        BindTowerFigureHandlerSystem();
        BindDropZoneSystem();

        BindDraggingService();

        BindGameFactory();

        RegisterMessagePipe();

        BindSaveLoadService();
    }

    void BindDraggingSystem()
    {
        Container.Bind<DraggingFigureSystem>()
                     .AsSingle()
                     .NonLazy();
    }

    void BindTowerFigureHandlerSystem()
    {
        Container.Bind<TowerFigureHandlerSystem>()
                     .AsSingle()
                     .NonLazy();
    }

    void BindDropZoneSystem()
    {
        Container.Bind<DropZoneSystem>()
                     .AsSingle()
                     .NonLazy();
    }

    private void BindDraggingService()
    {
        Container.Bind<IDraggingService>()
            .To<DraggingService>()
            .AsSingle()
            .NonLazy();
    }

    private void BindGameFactory()
    {
        Container.Bind<GameFactory>()
                 .AsSingle()
                 .NonLazy();
    }

    private void RegisterMessagePipe()
    {
        messagePipeOptions = Container.BindMessagePipe();
        GlobalMessagePipe.SetProvider(Container.AsServiceProvider());
        GetType().Assembly.GetTypes().ForEach(x =>
        {
            if (!x.IsAbstract || !x.IsSealed) return;

            System.Reflection.MethodInfo methodInfo = x.GetMethod("Install");

            if (methodInfo is null)
                return;

            System.Reflection.ParameterInfo[] parameterInfos = methodInfo.GetParameters();

            if (parameterInfos.Length != 2)
                return;

            if (parameterInfos[0].ParameterType != typeof(DiContainer) || parameterInfos[1].ParameterType != typeof(MessagePipeOptions))
                return;

            methodInfo.Invoke(null, new object[] { Container, messagePipeOptions });
        });
    }

    private void BindSaveLoadService()
    {
        Container.Bind<ISaveLoadProgressService>()
            .To<SaveLoadProgressService>()
            .AsSingle()
            .NonLazy();
    }
}