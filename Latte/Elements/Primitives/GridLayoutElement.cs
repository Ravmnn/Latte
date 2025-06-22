using System;
using System.Collections;
using System.Collections.Generic;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;
using Latte.Exceptions.Element;


namespace Latte.Elements.Primitives;


// TODO: code cleanup in here


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
        : base(parent, position, new Vec2f())
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


    protected GridLayoutCell FindAvailableCell()
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
        GridLayoutCell[,] oldCells = Cells;

        Cells = new GridLayoutCell[Rows, Columns];

        for (uint row = 0; row < Rows; row++)
        for (uint col = 0; col < Columns; col++)
        {
            InitializeCellBasedOnOldCellMatrix(oldCells, row, col);
            UpdateCellGeometry(row, col);
        }

        Size.Value = new Vec2f(Columns * CellWidth, Rows * CellHeight);

        RecreationRequired = false;
    }

    private void InitializeCellBasedOnOldCellMatrix(GridLayoutCell[,] oldCells, uint row, uint col)
    {
        if (AreIndicesInsideMatrixBounds(oldCells, row, col))
            Cells[row, col] = oldCells[row, col];
        else
            Cells[row, col] = new GridLayoutCell(this, new Vec2f(), new Vec2f());
    }

    private void UpdateCellGeometry(uint row, uint col)
    {
        Cells[row, col].RelativePosition.Value = new Vec2f(col * CellWidth, row * CellHeight);
        Cells[row, col].Size.Value = new Vec2f(CellWidth, CellHeight);
    }


    private static bool AreIndicesInsideMatrixBounds<T>(T[,] matrix, uint row, uint col)
        => row < matrix.GetLength(0) && col < matrix.GetLength(1);
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
