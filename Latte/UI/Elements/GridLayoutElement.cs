using System;
using System.Collections;
using System.Collections.Generic;

using Latte.Core.Type;
using Latte.Application;
using Latte.UI.Elements.Attributes;
using Latte.UI.Elements.Exceptions;


namespace Latte.UI.Elements;




// TODO: rebuild grid system from scratch... maybe use something like Horizontal and VerticalLayout

public enum GridLayoutGrowDirection
{
    Horizontal,
    Vertical
}




[ChildrenType(typeof(GridLayoutCellElement))]
public class GridLayoutElement : RectangleElement, IEnumerable<Element?>
{
    public GridLayoutCellElement[,] Cells { get; private set; }




    private uint _rows;
    public uint Rows
    {
        get => _rows;
        set
        {
            if (MaxRows is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxRows.Value, nameof(value));

            if (MinRows is not null)
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinRows.Value, nameof(value));

            _rows = value;
            RecreationRequired = true;
        }
    }


    private uint _columns;
    public uint Columns
    {
        get => _columns;
        set
        {
            if (MaxColumns is not null)
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxColumns.Value, nameof(value));

            if (MinColumns is not null)
                ArgumentOutOfRangeException.ThrowIfLessThan(value, MinColumns.Value, nameof(value));

            _columns = value;
            RecreationRequired = true;
        }
    }

    public uint? MaxRows { get; set; }
    public uint? MaxColumns { get; set; }

    public uint? MinRows { get; set; }
    public uint? MinColumns { get; set; }


    private float _cellWidth;
    public float CellWidth
    {
        get => _cellWidth;
        set
        {
            _cellWidth = value;
            RecreationRequired = true;
        }
    }


    private float _cellHeight;
    public float CellHeight
    {
        get => _cellHeight;
        set
        {
            _cellHeight = value;
            RecreationRequired = true;
        }
    }


    public GridLayoutGrowDirection GrowDirection { get; set; }
    public bool Fixed { get; set; }

    public bool RecreationRequired { get; set; }




    public GridLayoutElement(Element? parent, Vec2f? position, uint rows, uint columns, float cellWidth, float cellHeight)
        : base(parent, position, new Vec2f())
    {
        _rows = rows;
        _columns = columns;

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;

        GrowDirection = GridLayoutGrowDirection.Horizontal;

        Color = SFML.Graphics.Color.Transparent;

        Cells = new GridLayoutCellElement[0, 0];
        CreateCells();
    }




    public override void UnconditionalUpdate()
    {
        if (RecreationRequired)
            CreateCells();

        base.UnconditionalUpdate();
    }




    public void AddElementAtEnd(Element element)
        => FindAvailableCell().Element = element;




    public Element RemoveFirstElement()
    {
        var element = FindFirstElement() ?? throw new EmptyGridException();
        element.Parent = null;

        return element;
    }

    public Element RemoveLastElement()
    {
        var element = FindLastElement() ?? throw new EmptyGridException();
        element.Parent = null;

        return element;
    }


    public Element RemoveElementAt(uint row, uint column)
    {
        var cell = Cells[row, column];

        if (cell.Element is null)
            throw new IndexOutOfRangeException();

        var element = cell.Element;
        cell.Element = null;

        return element;
    }




    public void DeleteFirstElement() => App.RemoveElement(RemoveFirstElement());
    public void DeleteLastElement() => App.RemoveElement(RemoveLastElement());
    public void DeleteElementAt(uint row, uint column) => App.RemoveElement(RemoveElementAt(row, column));


    public void Clear()
    {
        for (uint row = 0; row < Rows; row++)
            for (uint col = 0; col < Columns; col++)
                DeleteElementAt(row, col);

        Rows = MinRows ?? 0;
        Columns = MinColumns ?? 0;

        CreateCells();
    }




    public Element? GetElementAt(uint row, uint col) => Cells[row, col].Element;

    public Element? this[uint row, uint col] => GetElementAt(row, col);




    public Element? FindFirstElement()
    {
        for (var row = 0; row < Cells.GetLength(0) - 1; row++)
        for (var col = 0; col < Cells.GetLength(1); col++)
            if (Cells[row, col].Element is { } element)
                return element;

        return null;
    }


    public Element? FindLastElement()
    {
        for (var row = Cells.GetLength(0) - 1; row >= 0; row--)
        for (var col = Cells.GetLength(1) - 1; col >= 0; col--)
            if (Cells[row, col].Element is { } element)
                return element;

        return null;
    }




    public IEnumerator<Element?> GetEnumerator() => new GridLayoutEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();




    protected GridLayoutCellElement FindAvailableCell()
    {
        foreach (var cell in Cells)
            if (cell.Element is null)
                return cell;

        GrowLayout();

        return FindAvailableCell();
    }




    protected void GrowLayout()
    {
        if (Fixed)
            return;

        switch (GrowDirection)
        {
            case GridLayoutGrowDirection.Horizontal:
                Columns++;
                break;

            case GridLayoutGrowDirection.Vertical:
                Rows++;
                break;
        }

        CreateCells();
    }




    protected void CreateCells()
    {
        var oldCells = Cells;

        Cells = new GridLayoutCellElement[Rows, Columns];

        for (uint row = 0; row < Rows; row++)
        for (uint col = 0; col < Columns; col++)
        {
            InitializeCellBasedOnOldCellMatrix(oldCells, row, col);
            UpdateCellGeometry(row, col);
        }

        Size = new Vec2f(Columns * CellWidth, Rows * CellHeight);

        RecreationRequired = false;
    }


    private void InitializeCellBasedOnOldCellMatrix(GridLayoutCellElement[,] oldCells, uint row, uint col)
    {
        if (AreIndicesInsideMatrixBounds(oldCells, row, col))
            Cells[row, col] = oldCells[row, col];
        else
            Cells[row, col] = new GridLayoutCellElement(this, new Vec2f(), new Vec2f());
    }


    private void UpdateCellGeometry(uint row, uint col)
    {
        Cells[row, col].RelativePosition = new Vec2f(col * CellWidth, row * CellHeight);
        Cells[row, col].Size = new Vec2f(CellWidth, CellHeight);
    }




    private static bool AreIndicesInsideMatrixBounds<T>(T[,] matrix, uint row, uint col)
        => row < matrix.GetLength(0) && col < matrix.GetLength(1);
}
