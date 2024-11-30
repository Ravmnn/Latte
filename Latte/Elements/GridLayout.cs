using System.Collections.Generic;
using System.Threading;
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
            _rows = value;
            RecreationRequired = true;
        }
    }

    public uint Columns
    {
        get => _columns;
        set
        {
            _columns = value;
            RecreationRequired = true;
        }
    }

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
    
    protected bool RecreationRequired { get; set; }
    

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
    {
        element.Parent = FindAvailableCell();
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
        
        // TODO: clean all this mess
        
        for (int row = 0; row < Rows; row++)
        for (int col = 0; col < Columns; col++)
        {
            Cells[row, col] = row < oldCells.GetLength(0) && col < oldCells.GetLength(1) && oldCells[row, col] is not null ? oldCells[row, col] : new(this, new(), new());
        }
        
        for (int row = 0; row < Rows; row++)
        for (int col = 0; col < Columns; col++)
        {
            List<Element> cellChildren = [];
            
            if (row < oldCells.GetLength(0) && col < oldCells.GetLength(1) && oldCells[row, col] is {} oldCell)
                cellChildren = oldCell.Children;
            
            Cells[row, col]!.RelativePosition.Value = new(col * ColumnSpacing, row * RowSpacing);
            Cells[row, col]!.Size.Value = new(ColumnSpacing, RowSpacing);
            
            cellChildren.ForEach(child => child.Parent = Cells[row, col]);
        }

        Size.Value = new(Columns * ColumnSpacing, Rows * RowSpacing);
        
        RecreationRequired = false;
    }
}