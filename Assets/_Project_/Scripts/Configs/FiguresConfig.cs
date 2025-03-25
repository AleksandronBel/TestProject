using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FiguresConfig", menuName = "ScriptableObjects/FiguresConfig", order = 3)]
public class FiguresConfig : ScriptableObject
{
    [SerializeField] private List<Figure> _figures;

    public List<Figure> Figures => _figures;
}

[Serializable]
public class Figure
{
    [SerializeField] private BaseFigureView _figureViewPrefab;
    [SerializeField] private List<SpriteId> _sprites;

    public BaseFigureView FigureView => _figureViewPrefab;
    public List<SpriteId> Sprites => _sprites;
}

[Serializable]
public class SpriteId
{
    public Sprite Sprite;
    public string Id = Guid.NewGuid().ToString();
}