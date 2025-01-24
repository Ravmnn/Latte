using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class CheckBoxSelectedIndicatorElement : RectangleElement
{
    public new CheckBoxElement Parent => (base.Parent as CheckBoxElement)!;


    public CheckBoxSelectedIndicatorElement(CheckBoxElement parent) : base(parent, new(), new())
    {
        IgnoreMouseInput = true;

        Alignment.Set(Elements.Alignment.Center);
        SizePolicy.Set(SizePolicyType.FitParent);
        SizePolicyMargin.Set(new(5f, 5f));

        Color.Set(new(50, 50, 50));
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


    public CheckBoxElement(Element? parent, Vec2f position, bool selected = false) : base(parent, position, new(20, 20), null)
    {
        SelectedIndicator = new CheckBoxSelectedIndicatorElement(this);

        Selected = selected;

        Radius.Set(5f);
        BorderSize.Set(2f);

        Down["Scale"] = new Vec2f(0.9f, 0.9f);

        UseDefaultAnimation = false;
    }


    public override void OnMouseClick()
    {
        base.OnMouseClick();
        Selected = !Selected;
    }
}
