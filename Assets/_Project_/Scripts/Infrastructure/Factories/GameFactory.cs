using System;
using UnityEngine;
using Zenject;

public class GameFactory
{
    readonly DiContainer container;

    public GameFactory(DiContainer container)
    {
        this.container = container;
    }
    /// <summary>
    /// Creates an instance of provided unity object with additional args
    /// </summary>
    /// <typeparam name="T">Unity component type to resolve</typeparam>
    /// <param name="prefab">prefab to instantiate</param>
    /// <param name="parent">optional parenting transform</param>
    /// <param name="args">additional dependencies for creating object</param>
    /// <returns></returns>
    public T Instantiate<T>(T prefab, Transform parent, Action<DiContainer> extraArgs = null) where T : MonoBehaviour =>
        Instantiate<T>(prefab.gameObject, parent, extraArgs);

    /// <summary>
    /// Creates an instance of provided unity object with additional args
    /// </summary>
    /// <typeparam name="T">Unity component type to resolve</typeparam>
    /// <param name="prefab">prefab to instantiate</param>
    /// <param name="parent">optional parenting transform</param>
    /// <param name="args">additional dependencies for creating object</param>
    /// <returns></returns>
    public T Instantiate<T>(GameObject prefab, Transform parent, Action<DiContainer> extraArgs = null)
    {
        DiContainer subContainer = container.CreateSubContainer();
        extraArgs?.Invoke(subContainer);
        T instance = subContainer.InstantiatePrefabForComponent<T>(prefab, parent);
        subContainer.Inject(instance);  
        return instance;
    }

    /// <summary>
    /// Creates an instance of provided type with additional args
    /// </summary>
    /// <typeparam name="T">type to be created</typeparam>
    /// <param name="args">additional dependencies for creating type</param>
    /// <returns></returns>
    public T Create<T>(Action<DiContainer> extraArgs = null)
    {
        DiContainer diContainer = container.CreateSubContainer();
        extraArgs?.Invoke(diContainer);
        return diContainer.Instantiate<T>();
    }

    /// <summary>
    /// Provides a way for reusing unity objects by reinjecting with another args
    /// 
    /// Warning! Be careful with object setup and test It
    /// </summary>
    /// <typeparam name="T">Unity component type</typeparam>
    /// <param name="component">Unity component to ReInject</param>
    /// <param name="args">additional dependencies to reinject into object</param>
    /// <returns></returns>

    public void Inject(GameObject gameObject, Action<DiContainer> extraArgs)
    {
        DiContainer subContainer = container.CreateSubContainer();
        extraArgs?.Invoke(subContainer);
        subContainer.InjectGameObject(gameObject);
    }
}