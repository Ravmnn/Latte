using System;

using Latte.Core.Type;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements;


public class GridLayoutCell : RectangleElement
{
    public GridLayoutCell(Element? parent, Vec2f position, Vec2f size) : base(parent, position, size)
    {
        Color.Value = SFML.Graphics.Color.Transparent;
    }
}

// TODO: add ability to change size of the grid after construction


[CanOnlyHaveChildOfType(typeof(GridLayoutCell))]
public class GridLayout : RectangleElement
{
    public GridLayoutCell[,] Cells { get; }
    
    public uint Rows { get; }
    public uint Columns { get; }
    
    public float RowSpacing { get; }
    public float ColumnSpacing { get; }
    
    
    public GridLayout(Element? parent, Vec2f position, uint rows, uint columns, float rowSpacing, float columnSpacing)
        : base(parent, position, new())
    {
        Rows = rows;
        Columns = columns;
        
        RowSpacing = rowSpacing;
        ColumnSpacing = columnSpacing;
        
        Cells = new GridLayoutCell[Rows, Columns];
        
        Color.Value = SFML.Graphics.Color.Transparent;
        
        CreateCells();
    }


    public void AddElement(Element element)
    {
        element.Parent = FindAvailableCell();
    }


    protected GridLayoutCell FindAvailableCell()
    {
        foreach (GridLayoutCell cell in Cells)
            if (cell.Children.Count == 0)
                return cell;
        
        throw new InvalidOperationException("Grid layout overflowed.");
    }
    

    protected void CreateCells()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                Cells[row, col] = new GridLayoutCell(this, new(col * ColumnSpacing, row * RowSpacing), new(ColumnSpacing, RowSpacing));

        Size.Value = new(Columns * ColumnSpacing, Rows * RowSpacing);
    }
}