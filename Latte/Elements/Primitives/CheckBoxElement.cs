using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class CheckBoxElement : ButtonElement
{
    protected RectangleElement SelectedArea { get; set; }

    public bool Selected { get; set; }


    public CheckBoxElement(Element? parent, Vec2f position, bool selected = false) : base(parent, position, new(20, 20), null)
    {
        // TODO: move to own class
        SelectedArea = new(this, new(), new())
        {
            Alignment = { Value = Elements.Alignment.Center },
            SizePolicy = { Value = SizePolicyType.FitParent },
            SizePolicyMargin = { Value = new(5f, 5f) },
            Color = { Value = new(50, 50, 50) },

            IgnoreMouseInput = true
        };

        Selected = selected;

        Radius.Set(5f);
        BorderSize.Set(2f);

        Down["Scale"] = new Vec2f(0.9f, 0.9f);

        UseDefaultAnimation = false;
    }


    public override void Update()
    {
        SelectedArea.Visible = Selected;
        SelectedArea.Radius.Set(Radius);

        base.Update();
    }


    public override void OnMouseUp()
    {
        base.OnMouseUp();
        Selected = !Selected;
    }
}
