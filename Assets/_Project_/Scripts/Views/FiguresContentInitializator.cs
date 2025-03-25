using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FiguresContentInitializator : MonoBehaviour
{ 
    private IFigureProvider _figureProvider;

    [SerializeField] private Transform _contentParent;
    [SerializeField] private Canvas _mainCanvas;

    private List<BaseFigureView> _views = new();

    private GameFactory _gameFactory;

    [Inject]
    private void Construct(GameFactory gameFactory, IFigureProvider figureProvider)
    {
        _gameFactory = gameFactory;

        _figureProvider = figureProvider;
        CreateFigures();
    }

    private void CreateFigures()
    {
        foreach (var figureGroup in _figureProvider.GetFigures())
        {
            foreach (var figureSprite in figureGroup.Sprites)
            {
                var figure = _gameFactory.Instantiate(figureGroup.FigureView, _contentParent);
                figure.ImageFigureColor.sprite = figureSprite.Sprite;
                figure.MainCanvas = _mainCanvas;
                _views.Add(figure);
            }
        }
    }
}
