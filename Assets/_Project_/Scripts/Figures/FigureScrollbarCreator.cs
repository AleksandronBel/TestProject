using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FigureScrollbarCreator : MonoBehaviour
{
    private IFigureProvider _figureProvider;

    [SerializeField] private Transform _contentTrasform;
    [SerializeField] private Canvas _mainCanvas;

    private List<FigureView> _views = new();

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
                var figure = _gameFactory.Instantiate(figureGroup.FigureView, _contentTrasform);
                figure.ImageFigureColor.sprite = figureSprite;
                figure.MainCanvas = _mainCanvas;
                _views.Add(figure);
            }
        }
    }
}
