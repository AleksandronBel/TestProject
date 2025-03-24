using R3;
using UnityEngine;

public class FigureModel
{
    public ReactiveProperty<Color> CubeColor { get; private set; }
    public ReactiveProperty<Vector3> Position { get; private set; }
    public FigureModel(Color color, Vector3 startPosition)
    {
        CubeColor = new ReactiveProperty<Color>(color);
        Position = new ReactiveProperty<Vector3>(startPosition);
    }
}
