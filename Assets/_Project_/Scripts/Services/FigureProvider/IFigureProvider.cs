using System.Collections.Generic;

public interface IFigureProvider
{
    public List<Figure> GetFigures();
    BaseFigureView GetFigureById(string spriteId);
}