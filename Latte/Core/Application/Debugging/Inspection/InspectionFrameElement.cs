using Latte.Core.Type;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Core.Application.Debugging.Inspection;


public class InspectionFrameElement : RectangleElement
{
    private InspectionData _data;


    public TextElement NameText { get; }
    public TextElement DataText { get; }

    public InspectionData Data
    {
        get => _data;
        set
        {
            _data = value;

            NameText.Text.Set(_data.Name);
            DataText.Text.Set(_data.Data);
        }
    }


    public InspectionFrameElement(InspectionData data)
        : base(null, new Vec2f(), new Vec2f(350, 350), 5)
    {
        _data = data;

        NameText = new TextElement(this, new Vec2f(), 10, data.Name)
        {
            Alignment = { Value = Elements.Alignment.HorizontalCenter | Elements.Alignment.Top },
            AlignmentMargin = { Value = new Vec2f(0, 10) }
        };

        DataText = new TextElement(this, new Vec2f(5, 20), 13, data.Data);

        Alignment.Set(Elements.Alignment.Center);

        Color.Set(new ColorRGBA(100, 100, 100, 150));
    }
}
