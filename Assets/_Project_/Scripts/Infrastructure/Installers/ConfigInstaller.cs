using UnityEngine;
using Zenject;

public class ConfigInstaller : MonoInstaller
{
    [SerializeField] private FiguresConfig _figuresConfig;

    public override void InstallBindings()
    {
        BindFigureProvider();
    }

    private void BindFigureProvider()
    {
        Container.Bind<IFigureProvider>()
            .To<ScriptableFigureProvider>()
            .AsSingle()
            .WithArguments(_figuresConfig);
    }
}
