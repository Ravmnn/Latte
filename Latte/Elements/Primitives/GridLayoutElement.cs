using System;
using System.Collections;
using System.Collections.Generic;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


[ChildrenAmount(1)]
public class GridLayoutCell : RectangleElement
{
    public Element? Element
    {
        get => Children.Count == 0 ? null : Children[0];
        set
        {
            if (Element is not null)
                Element.Parent = null;

            if (value is not null)
                value.Parent = this;
        }
    }


    public GridLayoutCell(GridLayoutElement parent, Vec2f position, Vec2f size) : base(parent, position, size)
    {
        Color.Value = SFML.Graphics.Color.Transparent;
    }
}


public enum GridLayoutGrowDirection
{
    Horizontal,
    Vertical
}


[ChildrenType(typeof(GridLayoutCell))]
public class GridLayoutElement : RectangleElement, IEnumerable<Element?>
{
    private uint _rows;
    private uint _columns;
    private float _cellHeight;
    private float _cellWidth;


    public GridLayoutCell[,] Cells { get; private set; }

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

    public float CellWidth
    {
        get => _cellWidth;
        set
        {
            _cellWidth = value;
            RecreationRequired = true;
        }
    }

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


    public GridLayoutElement(Element? parent, Vec2f position, uint rows, uint columns, float cellWidth, float cellHeight)
        : base(parent, position, new())
    {
        _rows = rows;
        _columns = columns;

        _cellWidth = cellWidth;
        _cellHeight = cellHeight;

        GrowDirection = GridLayoutGrowDirection.Horizontal;

        Color.Value = SFML.Graphics.Color.Transparent;

        Cells = new GridLayoutCell[0, 0];
        CreateCells();
    }


    public override void ConstantUpdate()
    {
        if (RecreationRequired)
            CreateCells();

        base.ConstantUpdate();
    }


    public void AddElementAtEnd(Element element)
        => FindAvailableCell().Element = element;


    public Element RemoveFirstElement()
    {
        Element element = FindFirstElement() ?? throw EmptyGridException();
        element.Parent = null;

        return element;
    }

    public Element RemoveLastElement()
    {
        Element element = FindLastElement() ?? throw EmptyGridException();
        element.Parent = null;

        return element;
    }


    public Element RemoveElementAt(uint row, uint column)
    {
        GridLayoutCell cell = Cells[row, column];

        if (cell.Element is null)
            throw new InvalidOperationException($"No element at row {row} and column {column}.");

        Element element = cell.Element;
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
        for (int row = 0; row < Cells.GetLength(0) - 1; row++)
        for (int col = 0; col < Cells.GetLength(1); col++)
            if (Cells[row, col].Element is { } element)
                return element;

        return null;
    }

    public Element? FindLastElement()
    {
        for (int row = Cells.GetLength(0) - 1; row >= 0; row--)
        for (int col = Cells.GetLength(1) - 1; col >= 0; col--)
            if (Cells[row, col].Element is { } element)
                return element;

        return null;
    }


    public IEnumerator<Element?> GetEnumerator() => new GridLayoutEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    protected GridLayoutCell FindAvailableCell()
    {
        foreach (GridLayoutCell cell in Cells)
            if (cell.Element is null)
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
        GridLayoutCell[,] oldCells = Cells;

        Cells = new GridLayoutCell[Rows, Columns];

        for (uint row = 0; row < Rows; row++)
        for (uint col = 0; col < Columns; col++)
        {
            InitializeCellBasedOnOldCellMatrix(oldCells, row, col);
            UpdateCellGeometry(row, col);
        }

        Size.Value = new(Columns * CellWidth, Rows * CellHeight);

        RecreationRequired = false;
    }

    private void InitializeCellBasedOnOldCellMatrix(GridLayoutCell[,] oldCells, uint row, uint col)
    {
        if (AreIndicesInsideMatrixBounds(oldCells, row, col))
            Cells[row, col] = oldCells[row, col];
        else
        {
            Cells[row, col] = new(this, new(), new());

            // TODO: please, add a way to track element creation automatically
            App.AddElement(Cells[row, col]);
        }
    }

    private void UpdateCellGeometry(uint row, uint col)
    {
        Cells[row, col].RelativePosition.Value = new(col * CellWidth, row * CellHeight);
        Cells[row, col].Size.Value = new(CellWidth, CellHeight);
    }


    private static bool AreIndicesInsideMatrixBounds<T>(T[,] matrix, uint row, uint col)
        => row < matrix.GetLength(0) && col < matrix.GetLength(1);


    private static InvalidOperationException EmptyGridException() => new("The grid contains no elements.");
}


public class GridLayoutEnumerator : IEnumerator<Element?>
{
    private readonly GridLayoutElement _gridLayout;
    private uint _row, _col;

    public Element? Current => _gridLayout[_row, _col];
    object? IEnumerator.Current => Current;

    private bool _disposed;


    public GridLayoutEnumerator(GridLayoutElement gridLayout)
    {
        _gridLayout = gridLayout;
        _row = _col = 0;

        _disposed = false;
    }


    public bool MoveNext()
    {
        _col++;

        if (_col < _gridLayout.Columns)
            return true;

        _col = 0;
        _row++;

        return _row < _gridLayout.Rows;
    }

    public void Reset()
    {
        _row = _col = 0;
    }


    // an IEnumerator should implement the dispose pattern

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        // nothing to dispose...

        if (_disposed)
            return;

        if (disposing) {}

        _disposed = true;
    }


    ~GridLayoutEnumerator()
    {
        Dispose(false);
    }
}
