using System;
using Latte.Core.Type;


namespace Latte.UI.Elements;




internal class CheckBoxSelectedIndicatorElement : RectangleElement
{
    public new CheckBoxElement Parent => (base.Parent as CheckBoxElement)!;




    public CheckBoxSelectedIndicatorElement(CheckBoxElement parent) : base(parent, new Vec2f(), new Vec2f())
    {
        IgnoreMouseInput = true;

        Alignment = Alignment.Center;
        SizePolicy = SizePolicy.FitParent;
        SizePolicyMargin = new Vec2f(5f, 5f);

        Color = new ColorRGBA(50, 50, 50);
    }




    public override void Update()
    {
        Visible = Parent.Selected;
        Radius = Parent.Radius;

        base.Update();
    }
}




public class CheckBoxElement : ButtonElement
{
    protected Element SelectedIndicator { get; set; }


    private bool _selected;
    public bool Selected
    {
        get => _selected;
        set
        {
            _selected = value;
            OnSelectedChanged();
        }
    }


    public event EventHandler? SelectedChangedEvent;




    public CheckBoxElement(Element? parent, Vec2f? position, bool selected = false)
        : base(parent, position, new Vec2f(20, 20), null)
    {
        SelectedIndicator = new CheckBoxSelectedIndicatorElement(this);

        Selected = selected;

        Radius = 5f;
        BorderSize = 2f;
    }




    public override void OnMouseClick()
    {
        Selected = !Selected;
        base.OnMouseClick();
    }




    protected virtual void OnSelectedChanged()
        => SelectedChangedEvent?.Invoke(this, EventArgs.Empty);
}
