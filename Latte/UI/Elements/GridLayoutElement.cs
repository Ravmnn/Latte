using System.Collections.Generic;
using System.Linq;

using Latte.Core.Type;


namespace Latte.UI.Elements;




public class GridLayoutElement : VerticalLayoutElement
{
    public List<Element> Rows => Elements;


    public uint MaxWidth { get; set; }
    public uint? MaxHeight
    {
        get => MaxElementCount;
        set => MaxElementCount = value;
    }


    public new Vec2f Margin { get; set; }




    public GridLayoutElement(Element? parent, Vec2f? position, uint rowWidth) : base(parent, position)
    {
        MaxWidth = rowWidth;

        AddNewRow();
    }




    public override void UnconditionalUpdate()
    {
        UpdateMargins();
        RemoveEmptyRows();

        base.UnconditionalUpdate();
    }


    private void UpdateMargins()
    {
        base.Margin = Margin.Y;

        foreach (var row in Rows.Cast<HorizontalLayoutElement>())
            row.Margin = Margin.X;
    }


    private void RemoveEmptyRows()
    {
        foreach (var row in Rows.Cast<HorizontalLayoutElement>())
            if (row.Elements.Count == 0)
                Elements.Remove(row);
    }




    public override void Push(Element element)
    {
        var lastRow = (Elements.Last() as HorizontalLayoutElement)!;

        if (lastRow.Elements.Count + 1 <= MaxWidth)
            lastRow.Elements.Add(element);
        else
        {
            AddNewRow();
            Push(element);
        }
    }


    public override Element Pop()
    {
        var lastRow = (Elements.Last() as HorizontalLayoutElement)!;
        var element = lastRow.Elements.Last();

        lastRow.Elements.RemoveAt(lastRow.Elements.Count - 1);

        return element;
    }


    private void AddNewRow()
        => Elements.Add(new HorizontalLayoutElement(this, null));
}
