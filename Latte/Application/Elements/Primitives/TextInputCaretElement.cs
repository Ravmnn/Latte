using Latte.Core.Type;
using Latte.Application.Elements.Primitives.Shapes;
using Latte.Application.Elements.Properties;


namespace Latte.Application.Elements.Primitives;


public class TextInputCaretElement : RectangleElement
{
    private int _index;


    public new TextInputElement Parent => (base.Parent as TextInputElement)!;

    public int Index
    {
        get => _index;
        set
        {
            if (value < 0)
                return;

            if (value > Parent.Text.Text.Value.Length)
                return;

            _index = value;
        }
    }

    public AnimatableProperty<Float> HeightFactor { get; }


    public TextInputCaretElement(TextInputElement parent)
        : base(parent, new Vec2f(), new Vec2f(5, 30))
    {
        Color.Set(new ColorRGBA(0, 0, 0, 200));
        Alignment.Set(Behavior.Alignment.VerticalCenter);

        Size.Value.X = 2f;

        Visible = false;

        HeightFactor = new AnimatableProperty<Float>(this, nameof(HeightFactor), 1.35f);
    }


    public override void Update()
    {
        UpdateGeometry();

        base.Update();
    }

    private void UpdateGeometry()
    {
        var absoluteGeometry = Parent.Text.GetAbsoluteGeometryOfCharacter(Index);
        absoluteGeometry.Height *= HeightFactor.Value;

        AbsolutePosition = absoluteGeometry.Position;
        Size.Set(new Vec2f(Size.Value.X, absoluteGeometry.Height));
    }


    public void Advance() => Index++;
    public void Back() => Index--;
}
