using System;
using System.Collections.Generic;
using System.Linq;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


// TODO: add an attribute that limits the number of children an element can have
public class GridLayoutCell : RectangleElement
{
    // TODO: limit children amount to one

    public GridLayoutCell(GridLayoutElement parent, Vec2f position, Vec2f size) : base(parent, position, size)
    {
        Color.Value = SFML.Graphics.Color.Transparent;
    }
}


public enum GridLayoutGrowDirection
{
    Horizontally,
    Vertically
}


// TODO: implement IEnumerable and IEnumerator

[CanOnlyHaveChildOfType(typeof(GridLayoutCell))]
public class GridLayoutElement : RectangleElement
{
    private uint _rows;
    private uint _columns;
    private float _cellHeight;
    private float _cellWidth;


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


    public void AddElementAtEnd(Element element)
        => element.Parent = FindAvailableCell();


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
        GridLayoutCell cell = Cells[row, column] ?? throw new IndexOutOfRangeException("Invalid grid index.");

        if (cell.Children.Count <= 0)
            throw new InvalidOperationException($"No element at row {row} and column {column}.");

        Element element = cell.Children.First();
        element.Parent = null;

        return element;
    }


    public void DeleteFirstElement() => App.RemoveElement(RemoveFirstElement());
    public void DeleteLastElement() => App.RemoveElement(RemoveLastElement());
    public void DeleteElementAt(uint row, uint column) => App.RemoveElement(RemoveElementAt(row, column));

    public void DeleteAll()
    {
        for (uint row = 0; row < Rows; row++)
            for (uint col = 0; col < Columns; col++)
                DeleteElementAt(row, col);
    }


    // TODO: add basic element handling methods, like the one below:

    public Element? FindFirstElement()
    {
        for (int row = 0; row < Cells.GetLength(0) - 1; row++)
        for (int col = 0; col < Cells.GetLength(1); col++)
        {
            GridLayoutCell? cell = Cells[row, col];

            if (cell is not null && cell.Children.Count > 0)
                return cell.Children.First();
        }

        return null;
    }

    public Element? FindLastElement()
    {
        for (int row = Cells.GetLength(0) - 1; row >= 0; row--)
        for (int col = Cells.GetLength(1) - 1; col >= 0; col--)
        {
            GridLayoutCell? cell = Cells[row, col];

            if (cell is not null && cell.Children.Count > 0)
                return cell.Children.Last();
        }

        return null;
    }


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
        /* TODO: this class uses a nullable matrix due to the following code to avoid intellisense warnings.
                 try to use an attribute to disable those warnings in a better way */

        GridLayoutCell?[,] oldCells = Cells;

        Cells = new GridLayoutCell?[Rows, Columns];

        for (uint row = 0; row < Rows; row++)
        for (uint col = 0; col < Columns; col++)
        {
            InitializeCellBasedOnOldCellMatrix(oldCells, row, col, out List<Element> oldCellChildren);
            UpdateCellGeometry(row, col);

            oldCellChildren.ForEach(child => child.Parent = Cells[row, col]);
        }

        Size.Value = new(Columns * CellWidth, Rows * CellHeight);

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

        Cells[row, col]!.RelativePosition.Value = new(col * CellWidth, row * CellHeight);
        Cells[row, col]!.Size.Value = new(CellWidth, CellHeight);
    }


    private static bool AreIndicesInsideMatrixBounds<T>(T[,] matrix, uint row, uint col)
        => row < matrix.GetLength(0) && col < matrix.GetLength(1);



    private static InvalidOperationException EmptyGridException() => new("The grid contains no elements.");
}
