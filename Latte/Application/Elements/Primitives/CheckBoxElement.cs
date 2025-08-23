using Latte.Core.Type;
using Latte.Application.Elements.Primitives.Shapes;


namespace Latte.Application.Elements.Primitives;


internal class CheckBoxSelectedIndicatorElement : RectangleElement
{
    public new CheckBoxElement Parent => (base.Parent as CheckBoxElement)!;


    public CheckBoxSelectedIndicatorElement(CheckBoxElement parent) : base(parent, new Vec2f(), new Vec2f())
    {
        IgnoreMouseInput = true;

        Alignment.Set(Behavior.Alignment.Center);
        SizePolicy.Set(Behavior.SizePolicy.FitParent);
        SizePolicyMargin.Set(new Vec2f(5f, 5f));

        Color.Set(new ColorRGBA(50, 50, 50));
    }


    public override void Update()
    {
        Visible = Parent.Selected;
        Radius.Set(Radius);

        base.Update();
    }
}


public class CheckBoxElement : ButtonElement
{
    protected RectangleElement SelectedIndicator { get; set; }

    public bool Selected { get; set; }


    public CheckBoxElement(Element? parent, Vec2f? position, bool selected = false) : base(parent, position, new Vec2f(20, 20), null)
    {
        SelectedIndicator = new CheckBoxSelectedIndicatorElement(this);

        Selected = selected;

        Radius.Set(5f);
        BorderSize.Set(2f);
    }


    public override void OnMouseClick()
    {
        base.OnMouseClick();
        Selected = !Selected;
    }
}
