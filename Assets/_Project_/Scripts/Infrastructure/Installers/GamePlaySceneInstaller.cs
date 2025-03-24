using MessagePipe;
using UnityEngine;
using Zenject;

public class GamePlaySceneInstaller : MonoInstaller
{
    [SerializeField] private FiguresSO _figuresSO;
    [SerializeField] private SpritesSO _spritesSO;

    MessagePipeOptions messagePipeOptions;

    public override void InstallBindings()
    {
        BindSaveLoadService();

        BindSpritesConfig();
        BindFigureProvider();
        
        BindFigureService();
        BindTowerService();

        RegisterMessagePipe();
        BindGameFactory();
    }

    private void BindSaveLoadService()
    { 
        Container.Bind<ISaveLoadProgressService>()
            .To<SaveLoadProgressService>()
            .AsSingle()
            .NonLazy();
    }

    private void BindSpritesConfig()
    {
        Container.Bind<ISpriteService>()
            .To<SpriteService>()
            .AsSingle()
            .WithArguments(_spritesSO);
    }

    private void BindFigureProvider()
    {
        Container.Bind<IFigureProvider>()
            .To<ScriptableFigureProvider>()
            .AsSingle()
            .WithArguments(_figuresSO);
    }

    private void BindFigureService()
    {
        Container.Bind<IFigureService>()
            .To<FigureService>()
            .AsSingle()
            .NonLazy();
    }

    private void BindTowerService()
    {
        Container.Bind<ITowerService>()
            .To<TowerService>()
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
}