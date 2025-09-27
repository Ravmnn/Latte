using Latte.Core.Type;


namespace Latte.UI.Elements;




public class WindowCloseButtonElement : ButtonElement
{
    public new WindowElement Parent => (base.Parent as WindowElement)!;


    public WindowCloseButtonElement(WindowElement parent) : base(parent, new Vec2f(), new Vec2f(15, 15), null)
    {
        Color = new ColorRGBA(255, 100, 100);

        Alignment = Alignment.TopRight;
        AlignmentMargin = new Vec2f(-7, 8);
    }


    public override void OnMouseClick()
    {
        Parent.Close();
        base.OnMouseClick();
    }
}
