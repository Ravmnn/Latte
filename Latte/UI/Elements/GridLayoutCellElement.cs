using Latte.Core.Type;
using Latte.UI.Elements.Attributes;


namespace Latte.UI.Elements;


[ChildrenAmount(1)]
public class GridLayoutCellElement : RectangleElement
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


    public GridLayoutCellElement(GridLayoutElement parent, Vec2f? position, Vec2f size) : base(parent, position, size)
    {
        Color = SFML.Graphics.Color.Transparent;
    }
}
