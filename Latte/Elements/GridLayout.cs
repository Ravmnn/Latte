using System;
using System.Collections.Generic;

using Latte.Core.Type;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements;


public class GridLayoutCell : RectangleElement
{
    public GridLayoutCell(GridLayout parent, Vec2f position, Vec2f size) : base(parent, position, size)
    {
        Color.Value = SFML.Graphics.Color.Transparent;
    }
}


public enum GridLayoutGrowDirection
{
    Horizontally,
    Vertically
}


[CanOnlyHaveChildOfType(typeof(GridLayoutCell))]
public class GridLayout : RectangleElement
{
    private uint _rows;
    private uint _columns;
    private float _rowSpacing;
    private float _columnSpacing;

    
    public GridLayoutCell?[,] Cells { get; private set; }

    public uint Rows
    {
        get => _rows;
        set
        {
            if (MaxRows is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxRows.Value, nameof(value));

            _rows = value;
            RecreationRequired = true;
        }
    }

    public uint Columns
    {
        get => _columns;
        set
        {
            if (MaxColumns is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxColumns.Value, nameof(value));
            
            _columns = value;
            RecreationRequired = true;
        }
    }
    
    public uint? MaxRows { get; set; }
    public uint? MaxColumns { get; set; }

    public float RowSpacing
    {
        get => _rowSpacing;
        set
        {
            _rowSpacing = value;
            RecreationRequired = true;
        }
    }

    public float ColumnSpacing
    {
        get => _columnSpacing;
        set
        {
            _columnSpacing = value;
            RecreationRequired = true;
        }
    }
    
    public GridLayoutGrowDirection GrowDirection { get; set; }
    public bool Fixed { get; set; }
    
    public bool RecreationRequired { get; set; }
    

    public GridLayout(Element? parent, Vec2f position, uint rows, uint columns, float rowSpacing, float columnSpacing)
        : base(parent, position, new())
    {
        _rows = rows;
        _columns = columns;
        
        _rowSpacing = rowSpacing;
        _columnSpacing = columnSpacing;

        GrowDirection = GridLayoutGrowDirection.Horizontally;
        
        Cells = new GridLayoutCell[Rows, Columns];
        
        Color.Value = SFML.Graphics.Color.Transparent;
        
        CreateCells();
    }


    public override void Update()
    {
        if (RecreationRequired)
            CreateCells();

        base.Update();
    }


    public void AddElement(Element element)
        => element.Parent = FindAvailableCell();


    protected GridLayoutCell FindAvailableCell()
    {
        foreach (GridLayoutCell? cell in Cells)
            if (cell is not null && cell.Children.Count == 0)
                return cell;
        
        GrowLayout();

        return FindAvailableCell();
    }


    protected void GrowLayout()
    {
        if (Fixed)
            throw new InvalidOperationException("Layout is fixed and can't be resized.");
        
        switch (GrowDirection)
        {
            case GridLayoutGrowDirection.Horizontally:
                Columns++;
                break;
            
            case GridLayoutGrowDirection.Vertically:
                Rows++;
                break;
        }
        
        CreateCells();
    }
    

    protected void CreateCells()
    {
        GridLayoutCell?[,] oldCells = Cells;
        
        Cells = new GridLayoutCell[Rows, Columns];
        
        for (uint row = 0; row < Rows; row++)
        for (uint col = 0; col < Columns; col++)
        {
            InitializeCellBasedOnOldCellMatrix(oldCells, row, col, out List<Element> oldCellChildren);
            UpdateCellGeometry(row, col);

            oldCellChildren.ForEach(child => child.Parent = Cells[row, col]);
        }

        Size.Value = new(Columns * ColumnSpacing, Rows * RowSpacing);
        
        RecreationRequired = false;
    }

    private void InitializeCellBasedOnOldCellMatrix(GridLayoutCell?[,] oldCells, uint row, uint col, out List<Element> oldCellChildren)
    {
        if (AreIndicesInsideMatrixBounds(oldCells, row, col) && oldCells[row, col] is not null)
        {
            oldCellChildren = oldCells[row, col]!.Children;
            Cells[row, col] = oldCells[row, col];
        }
        else
        {
            oldCellChildren = [];
            Cells[row, col] = new(this, new(), new());
        }
    }

    private void UpdateCellGeometry(uint row, uint col)
    {
        if (Cells[row, col] is null)
            return;
        
        Cells[row, col]!.RelativePosition.Value = new(col * ColumnSpacing, row * RowSpacing);
        Cells[row, col]!.Size.Value = new(ColumnSpacing, RowSpacing);
    }


    private static bool AreIndicesInsideMatrixBounds<T>(T[,] matrix, uint row, uint col)
        => row < matrix.GetLength(0) && col < matrix.GetLength(1);
}