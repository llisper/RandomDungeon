using System.Collections.Generic;

public interface ICellSeparator
{
    bool SeparateCells(List<Cell> cells, int maxPass);
    bool SeparateCellsOnePass(List<Cell> cells);
}
