using MessagePipe;
using UnityEngine;
using Zenject;

public class GamePlaySceneInstaller : MonoInstaller
{
    [SerializeField] private FiguresConfig _figuresConfig;

    MessagePipeOptions messagePipeOptions;
    
    //В ProjectContext сделать сервисы BindFigureService(); BindTowerService(); RegisterMessagePipe(); BindGameFactory(); BindSaveLoadService(после того как инициализируем все штуки (после фабрики));
    //В MonoInstaller инициализировать только зависомости, связанные с монобехами 
    // Для конфигов сделать ConfigInstaller

    public override void InstallBindings()
    {
        BindSaveLoadService();
        BindDraggingService();
        BindFigureProvider();

        //BindTowerService();

        BindDraggingSystem();
        BindTowerFigureHandlerSystem();
        BindDropZoneSystem();

        RegisterMessagePipe();
        BindGameFactory();
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


    private void BindSaveLoadService()
    { 
        Container.Bind<ISaveLoadProgressService>()
            .To<SaveLoadProgressService>()
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

    private void BindFigureProvider()
    {
        Container.Bind<IFigureProvider>()
            .To<ScriptableFigureProvider>()
            .AsSingle()
            .WithArguments(_figuresConfig);
    }

   /* private void BindTowerService()
    {
        Container.Bind<ITowerService>()
            .To<FigurePlacementSystem>()
            .AsSingle()
            .NonLazy();
    }*/

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
}