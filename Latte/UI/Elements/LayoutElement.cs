using System.Collections.Generic;
using System.Linq;

using Latte.Core.Type;
using Latte.UI.Elements.Exceptions;


namespace Latte.UI.Elements;




public abstract class LayoutElement : RectangleElement
{
    public List<Element> Elements { get; protected set; }


    public uint? MaxElementCount { get; set; }




    public LayoutElement(Element? parent, Vec2f? position) : base(parent, position, new Vec2f())
    {
        Elements = [];
        ClipChildren = false; // TODO: clip not working as intended
        // TODO: do not margin the final element

        Color = SFML.Graphics.Color.Transparent;
    }




    public override void UnconditionalUpdate()
    {
        if (Elements.Count > MaxElementCount)
            throw new MaxLayoutElementCountExceededException();

        UpdateElements();

        base.UnconditionalUpdate();
    }


    protected abstract void UpdateElements();


    protected void UpdateElement(Element element, Vec2f currentPosition)
    {
        element.Parent = this;
        element.Alignment = Alignment.None;
        element.RelativePosition = currentPosition;
    }




    public virtual void Push(Element element)
        => Elements.Add(element);


    public virtual Element Pop()
    {
        var element = Elements.Last();
        Elements.Remove(element);

        return element;
    }
}
