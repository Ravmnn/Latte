using Latte.Core.Type;


namespace Latte.UI.Elements;




public class TextInputCaretElement : RectangleElement
{
    public new TextInputElement Parent => (base.Parent as TextInputElement)!;




    private int _index;
    public int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                return;

            if (value > Parent.Text.Text.Length)
                return;

            _index = value;
        }
    }


    public float HeightFactor { get; set; }




    public TextInputCaretElement(TextInputElement parent)
        : base(parent, new Vec2f(), new Vec2f(5, 30))
    {
        Color = new ColorRGBA(0, 0, 0, 200);
        Alignment = Alignment.VerticalCenter;

        Size.X = 2f;

        Visible = false;

        HeightFactor = 1.35f;
    }




    public override void Update()
    {
        UpdateGeometry();

        base.Update();
    }


    private void UpdateGeometry()
    {
        var absoluteGeometry = Parent.Text.GetAbsoluteGeometryOfCharacter(Index);
        absoluteGeometry.Height *= HeightFactor;

        AbsolutePosition = absoluteGeometry.Position;
        Size = new Vec2f(Size.X, absoluteGeometry.Height);
    }




    public void Advance() => Index++;
    public void Back() => Index--;
}
