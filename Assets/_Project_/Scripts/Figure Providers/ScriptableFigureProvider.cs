using System.Collections.Generic;

public class ScriptableFigureProvider : IFigureProvider
{
    private FiguresSO _figuresSO;

    public ScriptableFigureProvider(FiguresSO figuresSO)
    {
        _figuresSO = figuresSO;
    }

    public List<Figure> GetFigures()
    {
        return _figuresSO.Figures;
    }
}