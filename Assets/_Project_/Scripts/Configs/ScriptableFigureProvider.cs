using System.Collections.Generic;

public class ScriptableFigureProvider : IFigureProvider
{
    private FiguresConfig _figuresSO;

    public ScriptableFigureProvider(FiguresConfig figuresSO)
    {
        _figuresSO = figuresSO;
    }

    public List<Figure> GetFigures()
    {
        return _figuresSO.Figures;
    }
}