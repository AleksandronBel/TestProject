using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FiguresConfig", menuName = "ScriptableObjects/FiguresConfig", order = 3)]
public class FiguresSO : ScriptableObject
{
    [SerializeField] private List<Figure> _figures;

    public List<Figure> Figures => _figures;
}

[Serializable]
public class Figure
{
    [SerializeField] private FigureView _figureView;
    [SerializeField] private List<Sprite> _sprites;

    public FigureView FigureView => _figureView;
    public List<Sprite> Sprites => _sprites;
}