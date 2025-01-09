using Latte.Core.Type;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Elements.Primitives;


public class CheckBox : ButtonElement
{
    protected RectangleElement SelectedArea { get; set; }

    public bool Selected { get; set; }


    public CheckBox(Element? parent, Vec2f position, bool selected = false) : base(parent, position, new(20, 20), null)
    {
        SelectedArea = new(this, new(), new())
        {
            Alignment = { Value = Elements.Alignment.Center },
            SizePolicy = { Value = SizePolicyType.FitParent },
            SizePolicyMargin = { Value = new(5f, 5f) },
            Color = { Value = new(50, 50, 50) },

            BlocksMouseInput = false
        };

        Selected = selected;

        Radius.Set(5f);
        BorderSize.Set(2f);

        Down["Scale"] = new Vec2f(0.9f, 0.9f);

        UseDefaultAnimation = false;
    }


    public override void Update()
    {
        base.Update();

        SelectedArea.Visible = Selected;
        SelectedArea.Radius.Set(Radius);
    }


    public override void OnMouseUp()
    {
        base.OnMouseUp();
        Selected = !Selected;
    }
}
